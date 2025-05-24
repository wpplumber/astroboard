<template>
  <div data-test-id="alert" v-if="visible" class="tw-left-1/2 tw-transform tw--translate-x-1/2 tw-indicator tw-absolute tw-bottom tw-bottom-7 tw-max-w-sm tw-mx-auto tw-z-50">
    <i-tabler-circle-letter-x class="tw-indicator-item tw-rounded-full tw-bg-black tw-text-white tw-h-6 tw-w-6 tw-cursor-pointer" @click="closeAlert"/>
    <div :role="'alert'" :class="alertClasses">
      <i-tabler-info-circle-filled class="tw-h-6 tw-w-6"/>
      <span>{{ message }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';

// Props
interface Props {
  message: string;
  visible: boolean;
  duration?: number;
  type?: 'error' | 'info' | 'alert' | 'success';
}

const props = defineProps<Props>();
const emit = defineEmits(['close']);

// Local state
const visible = ref(props.visible);

// Watch prop changes
watch(() => props.visible, (newValue) => {
  visible.value = newValue;
  if (newValue === true && props.duration) {
    setTimeout(() => {
      closeAlert();
    }, props.duration);
  }
});

// Close Alert
const closeAlert = () => {
  visible.value = false;
  emit('close'); // Emit close event to parent
};

// Computed property to dynamically change alert color
const alertClasses = computed(() => {
  switch (props.type) {
    case 'error':
      return 'tw-max-w-sm tw-alert tw-bg-red-500';
    case 'info':
      return 'tw-max-w-sm tw-alert tw-bg-blue-500';
    case 'alert':
      return 'tw-max-w-sm tw-alert tw-bg-yellow-500';
    case 'success':
      return 'tw-max-w-sm tw-alert tw-bg-green-500';
    default:
      return 'tw-max-w-sm tw-alert tw-bg-slate-200'; // Default color
  }
});
</script>
