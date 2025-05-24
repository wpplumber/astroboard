<script setup lang="ts">
import { ref } from "vue";

const isFullScreen = ref(false);

const toggleFullScreen = () => {
  const div = document.getElementById("astroboard");
  const umbNav = document.getElementById("navigation");
  if (umbNav && div && isFullScreen.value) {
    umbNav.classList.remove("tw-hidden");
    div.classList.remove(
      "tw-fixed",
      "tw-top-0",
      "tw-left-0",
      "tw-w-full",
      "tw-h-full",
      "tw-z-[9999]",
    );
    div.classList.add("tw-relative", "tw-w-auto", "tw-h-auto");
  } else if (umbNav && div) {
    div.classList.remove("tw-relative", "tw-w-auto", "tw-h-auto");
    div.classList.add(
      "tw-fixed",
      "tw-top-0",
      "tw-left-0",
      "tw-w-full",
      "tw-h-full",
      "tw-z-[9999]",
    );
    umbNav.classList.add("tw-hidden");
  }
  isFullScreen.value = !isFullScreen.value;
};
</script>
<template>
  <div data-tg-order="9"
  data-tg-title="Switch Modes Easily 🔄" data-tg-tour="Use the floating button to toggle between modes anytime! This flexibility allows you to adapt your workspace to your needs, whether you're in a hurry or diving deep into your projects." class="tw-fixed tw-bottom-4 tw-right-4 tw-flex tw-flex-col tw-gap-1">
    <button
      @click.prevent="toggleFullScreen"
      class="tw-btn-sm tw-btn-circle tw-bg-yellow-500 tw-flex tw-items-center"
    >
      <i-tabler-arrows-maximize
        v-if="!isFullScreen"
        class="tw-w-4 tw-h-4 tw-mx-auto"
      />
      <i-tabler-arrows-minimize v-else class="tw-w-4 tw-h-4 tw-mx-auto" />
    </button>
  </div>
</template>

<style scoped></style>
