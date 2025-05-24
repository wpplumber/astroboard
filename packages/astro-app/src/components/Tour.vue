<template>
</template>

<script setup>
import "@sjmc11/tourguidejs/src/scss/tour.scss";
import { TourGuideClient } from "@sjmc11/tourguidejs/src/Tour";
import { tourStore } from '~/stores/tourStore';
import { computed, watch } from 'vue';
import { useStore } from '@nanostores/vue';
import { completeTour, isTourCompleted } from '~/stores/tourStore'; 

const tg = new TourGuideClient({ exitOnClickOutside: false });

const userTourStore = useStore(tourStore);
watch(() => userTourStore.value.cardsLoadedCount,
 (newValue) => {
  if (newValue === 5 && !isTourCompleted()) {
    tg.start();

    tg.onBeforeStepChange(() => {
      if (tg.activeStep === 4) {
        const link = document.querySelector('[data-test-id="license"]');
        if (link) link.click();
      }
      if (tg.activeStep === 5) {
        const link = document.querySelector('[data-test-id="feedback"]');
        if (link) link.click();
      }
    });

    tg.onFinish(()=>{
     completeTour();
    });
  }
});
</script>

