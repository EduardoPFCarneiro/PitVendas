<script setup lang="ts">
import { CircleXIcon, MailIcon } from 'vue-tabler-icons';
import { useAuthStore } from '@/stores/auth';

const auth = useAuthStore();

function sair() {
  auth.logout();
}
</script>

<template>
    <!-- ---------------------------------------------- -->

    <!-- ---------------------------------------------- -->
    <v-menu :close-on-content-click="true" class="profile_popup">
        <template v-slot:activator="{ props }">
            <div class="text-left px-0 cursor-pointer" variant="text" v-bind="props">
                <div class="d-flex align-center">
                    <v-avatar size="50" color="primary">
                        <img v-if="auth.usuario?.foto" :src="auth.usuario.foto" width="50" :alt="auth.usuario?.nome" />
                        <span v-else class="text-h6 font-weight-semibold">{{ auth.iniciais }}</span>
                    </v-avatar>
                    <div class="ml-md-4 d-md-block d-none">
                        <h6 class="text-h6 d-flex align-center text-black font-weight-semibold">
                            {{ auth.usuario?.nome }}
                        </h6>
                        <span class="text-subtitle-2 font-weight-medium text-grey100">Representante</span>
                    </div>
                </div>
            </div>
        </template>
        <v-sheet rounded="lg" width="385" elevation="10" class="mt-5">
            <div class="px-8 pt-6">
                <div class="d-flex align-center justify-space-between">
                    <h6 class="text-h5 font-weight-semibold">Meu perfil</h6>
                    <CircleXIcon size="22" class="text-grey100 cursor-pointer opacity-50" />
                </div>

                <div class="d-flex align-center mt-5 pb-6">
                    <v-avatar size="90" color="primary">
                        <img v-if="auth.usuario?.foto" :src="auth.usuario.foto" width="90" :alt="auth.usuario?.nome" />
                        <span v-else class="text-h4 font-weight-semibold">{{ auth.iniciais }}</span>
                    </v-avatar>
                    <div class="ml-5">
                        <h6 class="text-h5 mb-n1">{{ auth.usuario?.nome }}</h6>
                        <span class="text-subtitle-1 font-weight-regular text-grey100 font-weight-medium">
                            Representante
                        </span>
                        <div class="d-flex align-center mt-1">
                            <MailIcon size="18" stroke-width="1.5" class="text-grey100" />
                            <span class="text-subtitle-1 text-grey100 font-weight-medium ml-2">
                                {{ auth.usuario?.email }}
                            </span>
                        </div>
                    </div>
                </div>
                <v-divider></v-divider>
            </div>
            <div class="pa-6 text-center">
                <v-btn color="primary" size="large" rounded="pill" block @click="sair">
                    Sair
                </v-btn>
            </div>
        </v-sheet>
    </v-menu>
</template>
