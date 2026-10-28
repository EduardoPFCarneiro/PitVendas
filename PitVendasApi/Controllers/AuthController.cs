using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PitVendasApi.Dtos;
using PitVendasApi.Identity;
using PitVendasApi.Services;

namespace PitVendasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICurrentUser _usuarioAtual;
        private readonly IMapper _mapper;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _ambiente;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ICurrentUser usuarioAtual,
            IMapper mapper,
            IEmailService email,
            IConfiguration config,
            IWebHostEnvironment ambiente)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _usuarioAtual = usuarioAtual;
            _mapper = mapper;
            _email = email;
            _config = config;
            _ambiente = ambiente;
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenDto>> Registrar(RegistroDto dto)
        {
            var usuario = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.Telefone,
                Nome = dto.Nome
            };

            var resultado = await _userManager.CreateAsync(usuario, dto.Senha);

            if (!resultado.Succeeded)
            {
                foreach (var erro in resultado.Errors)
                    ModelState.AddModelError(string.Empty, erro.Description);

                return ValidationProblem(ModelState);
            }

            return Ok(MontarToken(usuario));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);

            if (usuario is null)
                return Unauthorized(new { mensagem = "E-mail ou senha invalidos." });

            if (!usuario.Ativo)
                return Unauthorized(new { mensagem = "Usuario inativo." });

            var resultado = await _signInManager.CheckPasswordSignInAsync(
                usuario, dto.Senha, lockoutOnFailure: true);

            if (resultado.IsLockedOut)
                return Unauthorized(new { mensagem = "Conta bloqueada temporariamente por excesso de tentativas." });

            if (!resultado.Succeeded)
                return Unauthorized(new { mensagem = "E-mail ou senha invalidos." });

            return Ok(MontarToken(usuario));
        }

        [HttpGet("eu")]
        [Authorize]
        public async Task<ActionResult<UsuarioDto>> Eu()
        {
            var usuario = await _userManager.FindByIdAsync(_usuarioAtual.Id!);

            if (usuario is null)
                return Unauthorized();

            return Ok(_mapper.Map<UsuarioDto>(usuario));
        }

        [HttpPost("esqueci-senha")]
        [AllowAnonymous]
        public async Task<ActionResult<EsqueciSenhaRespostaDto>> EsqueciSenha(
            EsqueciSenhaDto dto, CancellationToken ct)
        {
            var resposta = new EsqueciSenhaRespostaDto
            {
                Mensagem = "Se este e-mail estiver cadastrado, enviaremos as instruções de recuperação.",
            };

            var usuario = await _userManager.FindByEmailAsync(dto.Email);
            if (usuario is null || !usuario.Ativo)
                return Ok(resposta);

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var link = MontarLinkRedefinicao(usuario.Email!, token);

            if (_email.Disponivel)
            {
                try
                {
                    await _email.EnviarAsync(usuario.Email!,
                        "Recuperação de senha — Pit Vendas",
                        MontarCorpoRecuperacao(usuario.Nome, link),
                        ct: ct);
                }
                catch (RegraNegocioException)
                {
                }
            }

            if (_ambiente.IsDevelopment())
                resposta.LinkDesenvolvimento = link;

            return Ok(resposta);
        }

        [HttpPost("redefinir-senha")]
        [AllowAnonymous]
        public async Task<IActionResult> RedefinirSenha(RedefinirSenhaDto dto)
        {
            var usuario = await _userManager.FindByEmailAsync(dto.Email);

            const string invalido = "Link de recuperação inválido ou expirado. Solicite um novo.";

            if (usuario is null)
                return BadRequest(new { mensagem = invalido });

            var resultado = await _userManager.ResetPasswordAsync(usuario, dto.Token, dto.NovaSenha);

            if (!resultado.Succeeded)
            {
                var erroSenha = resultado.Errors.FirstOrDefault(e =>
                    e.Code.Contains("Password", StringComparison.OrdinalIgnoreCase));

                return BadRequest(new
                {
                    mensagem = erroSenha?.Description ?? invalido,
                });
            }

            return Ok(new { mensagem = "Senha redefinida com sucesso. Você já pode entrar." });
        }

        // ------------------------------------------------------------

        private string MontarLinkRedefinicao(string email, string token)
        {
            var baseUrl = _config["App:UrlBase"]?.TrimEnd('/') ?? "http://localhost:3000";
            return $"{baseUrl}/redefinir-senha" +
                   $"?email={Uri.EscapeDataString(email)}" +
                   $"&token={Uri.EscapeDataString(token)}";
        }

        private static string MontarCorpoRecuperacao(string nome, string link) => $"""
            <div style="font-family: Arial, sans-serif; color:#2A3547;">
              <h2 style="color:#0d86d9;">Recuperação de senha</h2>
              <p>Olá, {System.Net.WebUtility.HtmlEncode(nome)}.</p>
              <p>Recebemos um pedido para redefinir a sua senha no Pit Vendas.
                 Clique no botão abaixo para criar uma nova senha:</p>
              <p style="margin: 24px 0;">
                <a href="{link}"
                   style="background:#0d86d9; color:#fff; padding:12px 24px;
                          border-radius:6px; text-decoration:none; font-weight:bold;">
                  Redefinir minha senha
                </a>
              </p>
              <p style="color:#707a82; font-size:13px;">
                Se você não pediu isso, pode ignorar este e-mail — sua senha continua a mesma.
              </p>
            </div>
            """;

        private TokenDto MontarToken(ApplicationUser usuario)
        {
            var (token, expiraEm) = _tokenService.Gerar(usuario);

            return new TokenDto
            {
                Token = token,
                ExpiraEm = expiraEm,
                Usuario = _mapper.Map<UsuarioDto>(usuario)
            };
        }
    }
}
