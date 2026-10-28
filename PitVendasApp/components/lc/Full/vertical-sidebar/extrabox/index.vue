<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { useAuthStore } from '@/stores/auth';

const auth = useAuthStore();
</script>

<template>
    <v-sheet rounded="md" color="lightprimary" class="pa-4 ExtraBox hide-menu">
        <div class="d-flex align-center justify-space-between ga-3">
            <v-avatar size="50" color="primary">
                <img v-if="auth.usuario?.foto" :src="auth.usuario.foto" width="50" :alt="auth.usuario?.nome" />
                <span v-else class="text-h6 font-weight-semibold">{{ auth.iniciais }}</span>
            </v-avatar>
            <div class="flex-grow-1 overflow-hidden">
                <h6 class="text-h6 font-weight-semibold text-truncate">{{ auth.usuario?.nome }}</h6>
                <span class="text-subtitle-2 font-weight-medium text-grey100">Representante</span>
            </div>
            <div>
                <v-btn icon class="bg-lightprimary" flat size="small" title="Sair" @click="auth.logout()">
                    <Icon icon="solar:logout-linear" class="text-primary" stroke-width="3" height="24" width="24" />
                </v-btn>
            </div>
        </div>
    </v-sheet>
</template>

<style lang="scss">
.ExtraBox {
    position: relative;
}

.line-height-none {
    line-height: normal;
}
</style>
