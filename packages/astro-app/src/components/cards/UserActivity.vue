<template>
  <div class="tw-w-full tw-bg-white tw-rounded-lg tw-relative">
    <div
      v-if="!loading"
      class="tw-carousel tw-w-full tw-h-full tw-rounded-lg tw-overflow-y-hidden"
    >
      <div
        v-for="(activity, index) in activities"
        :key="index"
        class="tw-carousel-item tw-relative tw-w-full tw-rounded-lg"
        :class="{
          'tw-block': index === activeSlide,
          'tw-hidden': index !== activeSlide,
        }"
      >
        <div
          class="tw-bg-white tw-flex tw-flex-col tw-gap-y-2 tw-py-2 tw-items-center tw-h-full tw-w-full tw-my-auto tw-absolute tw--translate-x-1/2 tw--translate-y-1/2 tw-top-1/2 tw-left-1/2"
        >
          <div>
            <span
              :class="tagClasses(activity.status)"
              class="tw-text-xs tw-font-medium tw-me-2 tw-px-2.5 tw-py-0.5 tw-rounded-full"
              >{{ activity.status }}</span
            >
          </div>
          <span class="tw-font-bold">{{ activity.name }}</span>
          <div class="tw-flex tw-flex-col tw-justify-evenly tw-h-full">
            <div v-if="index === 0"
              data-tg-order="2"
             data-tg-title="Meet Your Collaborators 👥" data-tg-tour="Quickly spot your team members in the carousel! Hover over each avatar to reveal their full name and get to know who’s contributing to your projects."
              class="tw-flex tw-justify-center tw--space-x-4 rtl:tw-space-x-reverse tw-mt-2"
            >
              <div
                v-for="(collaborator, i) in activity.collaborators.slice(0, 5)"
                :key="collaborator"
                class="tw-tooltip"
                :data-tip="collaborator"
              >
                <div
                  class="tw-flex tw-items-center tw-justify-center tw-w-12 tw-h-12 tw-border-2 tw-border-white tw-rounded-full tw-bg-accent dark:tw-border-gray-800"
                >
                  {{ collaborator.split(" ")[0].charAt(0)
                  }}{{ collaborator.split(" ").slice(-1)[0].charAt(0) }}
                </div>
              </div>
              <div
                v-if="activity.collaborators.length > 5"
                class="tw-tooltip tw-tooltip-bottom"
                data-tip="remaining collaborator"
              >
                <div
                  class="tw-flex tw-items-center tw-justify-center tw-w-12 tw-h-12 tw-text-xs tw-font-medium tw-text-white tw-bg-gray-700 tw-border-2 tw-border-white tw-rounded-full hover:tw-bg-gray-600 dark:tw-border-gray-800  tw-mb-4"
                >
                  +{{ activity.collaborators.length - 5 }}
                </div>
              </div>
            </div>
            <div v-else
              class="tw-flex tw-justify-center tw--space-x-4 rtl:tw-space-x-reverse tw-mt-2"
            >
              <div
                v-for="(collaborator, i) in activity.collaborators.slice(0, 5)"
                :key="collaborator"
                class="tw-tooltip"
                :data-tip="collaborator"
              >
                <div
                  class="tw-flex tw-items-center tw-justify-center tw-w-12 tw-h-12 tw-border-2 tw-border-white tw-rounded-full tw-bg-accent dark:tw-border-gray-800"
                >
                  {{ collaborator.split(" ")[0].charAt(0)
                  }}{{ collaborator.split(" ").slice(-1)[0].charAt(0) }}
                </div>
              </div>
              <div
                v-if="activity.collaborators.length > 5"
                class="tw-tooltip tw-tooltip-bottom"
                data-tip="remaining collaborator"
              >
                <div
                  class="tw-flex tw-items-center tw-justify-center tw-w-12 tw-h-12 tw-text-xs tw-font-medium tw-text-white tw-bg-gray-700 tw-border-2 tw-border-white tw-rounded-full hover:tw-bg-gray-600 dark:tw-border-gray-800  tw-mb-4"
                >
                  +{{ activity.collaborators.length - 5 }}
                </div>
              </div>
            </div>

            <div class="tw-absolute tw-left-5 tw-bottom-2 tw-text-xs tw-flex tw-items-end tw-mt-1">
              <i-tabler-clock-edit class="tw-w-5 tw-h-5 tw-mr-1" />
              <span> {{ formatToFullTimestamp(activity.updateDate) }}</span>
            </div>
          </div>
        </div>
        <div
          class="tw-absolute tw-left-5 tw-right-5 tw-top-1/2 tw-flex tw--translate-y-1/2 tw-transform tw-justify-between"
        >
          <button @click="prevSlide" class="tw-btn tw-btn-circle">❮</button>
          <button @click="nextSlide" class="tw-btn tw-btn-circle">❯</button>
        </div>
      </div>
    </div>

    <div v-else class="tw-flex tw-w-full tw-flex-col tw-gap-2">
      <div class="tw-skeleton tw-h-16 tw-w-full"></div>
      <div class="tw-skeleton tw-h-3 tw-w-28"></div>
      <div class="tw-skeleton tw-h-3 tw-w-full"></div>
    </div>

    <h3 class="tw-font-bold tw-text-lg tw-absolute tw-left-3 tw-top-2">
      Activities
    </h3>

    <Menu as="div" class="tw-absolute tw-right-1 tw-top-1">
            <div>
              <MenuButton
                class="tw-inline-flex tw-items-center tw-justify-center tw-text-gray-500 tw-w-8 tw-h-8 dark:tw-text-gray-400 hover:tw-bg-gray-100 dark:hover:tw-bg-gray-700 focus:tw-outline-none focus:tw-ring-4 focus:tw-ring-gray-200 dark:focus:tw-ring-gray-700 tw-rounded-lg tw-text-sm"
              >
                <svg
                  class="tw-w-3.5 tw-h-3.5 tw-text-gray-800 dark:tw-text-white"
                  aria-hidden="true"
                  xmlns="http://www.w3.org/2000/svg"
                  fill="currentColor"
                  viewBox="0 0 16 3"
                >
                  <path
                    d="M2 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3Zm6.041 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3ZM14 0a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3Z"
                  />
                </svg>
                <span class="tw-sr-only">Open dropdown</span>
              </MenuButton>
            </div>

            <transition
              enter-active-class="tw-transition tw-duration-100 tw-ease-out"
              enter-from-class="tw-transform tw-scale-95 tw-opacity-0"
              enter-to-class="tw-transform tw-scale-100 tw-opacity-100"
              leave-active-class="tw-transition tw-duration-75 tw-ease-in"
              leave-from-class="tw-transform tw-scale-100 tw-opacity-100"
              leave-to-class="tw-transform tw-scale-95 tw-opacity-0"
            >
              <MenuItems
                class="tw-z-30 tw-absolute tw-right-0 tw-w-56 tw-origin-top-right tw-divide-y tw-divide-gray-100 tw-rounded-md tw-bg-white tw-shadow-lg tw-ring-1 tw-ring-black/5 focus:tw-outline-none"
              >
                <div class="tw-px-1 tw-py-1">
                  <MenuItem v-slot="{ active }">
                    <button
                    @click="selectItem('today')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      Today
                    </button>
                  </MenuItem>
                  <MenuItem v-slot="{ active }">
                    <button
                    @click="selectItem('yesterday')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      Yesterday
                    </button>
                  </MenuItem>
                  <MenuItem v-slot="{ active }">
                    <button
                    @click="selectItem('currentweek')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      This Week
                    </button>
                  </MenuItem>
                </div>
                <div class="tw-px-1 tw-py-1">
                  <MenuItem v-slot="{ active }">
                    <button
                    @click="selectItem('lastweek')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      Last 7 days
                    </button>
                  </MenuItem>
                  <MenuItem v-slot="{ active }">
                    <button
                      @click="selectItem('lastmonth')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      Last 30 days
                    </button>
                  </MenuItem>
                </div>
                <div class="tw-px-1 tw-py-1">
                  <MenuItem v-slot="{ active }">
                    <button
                    @click="selectItem('last90days')"
                      :class="[
                        active ? 'tw-bg-gray-100' : '',
                        'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                      ]"
                    >
                      Last 90 days
                    </button>
                  </MenuItem>
                </div>
              </MenuItems>
            </transition>
          </Menu>
    <div class="tw-absolute tw-bottom-2 tw-right-10 tw-text-[#e4e4e7] tw-font-bold tw-text-sm">{{selectedLabel}}</div>

  </div>
</template>

<script setup lang="ts">
import { Menu, MenuButton, MenuItem, MenuItems } from "@headlessui/vue";
import { formatToFullTimestamp } from "~/utils/dateUtils";
import { computed, onMounted, ref } from "vue";
import type { Activity } from "~/utils/interfaces.ts";
import { incrementCardsLoadedCount } from '~/stores/tourStore';

const props = defineProps({
  currentHost: {
    type: String,
    required: true
  }
});

const alertMessage = ref(`Enter license to unlock all features!`);
const emit = defineEmits();

function showAlert() {
  emit('showAlert', { msg: alertMessage.value, type: 'alert' });
}

const loading = ref(true);
const periodOptions = [
  { value: "currentweek", label: "Current Week" },
  { value: "today", label: "Today" },
  { value: "yesterday", label: "Yesterday" },
  { value: "lastweek", label: "Last Week" },
  { value: "last90days", label: "Last 90 Days" },
  { value: "lastmonth", label: "Last 30 Days" },
];
const selectedItem = ref("last90days");
const selectedLabel = computed(() => {
  return (
    periodOptions.find((option) => option.value === selectedItem.value)
      ?.label || ""
  );
});

const selectItem = async (item) => {
  selectedItem.value = item;
  await fetchData();
};

const activities = ref<Activity[]>();

const activeSlide = ref(0);

const nextSlide = () => {
  activeSlide.value = (activeSlide.value + 1) % activities.value.length;
};

const prevSlide = () => {
  activeSlide.value =
    (activeSlide.value - 1 + activities.value.length) % activities.value.length;
};

function tagClasses(tag: string) {
  const baseClasses = "tw-bg-gray-100 tw-text-gray-800";
  switch (tag) {
    case "Deleted":
      return "tw-bg-[#d42054] tw-text-white";
    case "Unpublished":
      return "tw-bg-[#ff9412] tw-text-white";
    case "Rollback":
      return "tw-bg-purple-100 tw-text-blue-800";
    case "Sent to Publish":
      return "tw-bg-pink-100 tw-text-blue-800";
    case "Published":
      return "tw-bg-[#2bc37c] tw-text-white";
    case "Saved":
      return "tw-bg-pink-100 tw-text-blue-800";
    case "Created":
      return "tw-bg-green-500 tw-text-white";
    default:
      return baseClasses;
  }
}

onMounted(async () => {
  await fetchData();
  incrementCardsLoadedCount();
});

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;

    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getuseractivity?period=${selectedItem.value}`,
    );
    const result = await response.json();

    // Update reactive data
    activities.value = result.pages;
    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

const formattedUpdateDate = (dateActivity: string) => {
  return formatToFullTimestamp(new Date(dateActivity), "PPpp");
};
</script>

<style scoped>
.active-slide {
  display: block;
}

/* .tw-carousel-item {
  display: none;
} */
</style>
