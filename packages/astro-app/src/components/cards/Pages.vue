<template>
  <div
    class="tw-h-full tw-flex tw-items-center tw-max-w-xs tw-w-full tw-bg-white tw-rounded-lg tw-shadow dark:tw-bg-gray-800 px-2 md:tw-px-3"
  >
    <div v-if="!loading" class="tw-grid tw-grid-cols-2 tw-gap-x-1 tw-h-full">
      <div class="tw-flex tw-flex-col tw-justify-evenly">
        <h5
          id="pages-title"
          class="tw-leading-none tw-text-lg tw-font-bold tw-text-gray-900 dark:tw-text-white tw-pb-2"
        >
          {{ title }}
        </h5>
        <span
          class="tw-text-3xl tw-font-bold tw-text-gray-900 dark:tw-text-white"
          v-html="formatValueWithK(value)"
        />
        <div
          :class="[
            rate > 0
              ? 'tw-text-green-500 dark:tw-text-green-500'
              : rate === 0
                ? 'tw-text-blue-500 dark:tw-text-blue-500'
                : 'tw-text-red-500 dark:tw-text-red-500',
            'tw-flex tw-items-center tw-px-2.5 tw-py-0.5 tw-text-sm tw-font-semibold tw-text-center',
          ]"
        >
          <i-tabler-arrow-up
            v-if="rate !== 0"
            :class="[rate >= 0 ? '' : 'tw-rotate-180', 'tw-w-5 tw-h-5 tw-ms-1']"
          />
          {{ rate }}%
        </div>
      </div>
      <div class="tw-relative tw-flex tw-flex-col tw-justify-end">
        <div
          data-tg-order="1"
          data-tg-title="Welcome to Your Dashboard! 🎉"
          data-tg-tour="Easily navigate through your data with the filtering options available on each card! Use the dropdown in the top-right corner of every card to filter by different time periods, ensuring you view the most relevant insights."
          class="tw-absolute tw-top-0 tw-right-0 tw-flex tw-justify-end tw-items-center"
        >
          <Menu as="div" class="tw-relative tw-inline-block tw-text-left">
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
        </div>
        <div ref="chartRef"></div>
      </div>
    </div>
    <div v-else class="tw-flex tw-w-full tw-flex-col tw-gap-2">
      <div class="tw-skeleton tw-h-16 tw-w-full"></div>
      <div class="tw-skeleton tw-h-3 tw-w-28"></div>
      <div class="tw-skeleton tw-h-3 tw-w-full"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Menu, MenuButton, MenuItem, MenuItems } from "@headlessui/vue";
import { useStore } from "@nanostores/vue";
import ApexCharts from "apexcharts";
import { computed, onMounted, ref, watch, watchEffect, defineEmits } from "vue";
import { formatValueWithK } from "~/utils/formatting";

const props = defineProps({
  currentHost: {
    type: String,
    required: true,
  },
});

const alertMessage = ref(`Enter license to unlock all features!`);
const emit = defineEmits();

function showAlert() {
  emit("showAlert", { msg: alertMessage.value, type: "alert" });
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

const title = ref("Pages");
const value = ref(0);
const rate = ref(0);

const data = ref([]);

const categories = ref([]);

const chartRef = ref(null);

const getChartOptions = ref({
  chart: {
    height: "80%",
    width: "100%",
    type: "area",
    fontFamily: "Inter, sans-serif",
    dropShadow: {
      enabled: false,
    },
    toolbar: {
      show: false,
    },
  },
  tooltip: {
    fixed: {
      enabled: true,
      position: "topLeft",
      offsetX: 0,
      offsetY: -10,
    },
    enabled: true,
    x: {
      show: false,
    },
  },
  fill: {
    type: "gradient",
    gradient: {
      opacityFrom: 0.55,
      opacityTo: 0,
      shade: rate.value >= 0 ? "#1C64F2" : "#FBD5D5",
      gradientToColors: rate.value >= 0 ? ["#1C64F2"] : ["#FBD5D5"],
    },
  },
  dataLabels: {
    enabled: false,
  },
  stroke: {
    width: 6,
  },
  grid: {
    show: false,
    strokeDashArray: 4,
    padding: {
      left: 2,
      right: 2,
      top: 0,
    },
  },
  series: [
    {
      name: "New pages",
      data: data.value,
      color: rate.value >= 0 ? "#1A56DB" : "#E02424",
    },
  ],
  xaxis: {
    categories: categories.value,
    labels: {
      show: false,
      style: {
        fontFamily: "Inter, sans-serif",
        cssClass: "text-xs font-normal fill-gray-500 dark:fill-gray-400",
      },
    },
    axisBorder: {
      show: false,
    },
    axisTicks: {
      show: false,
    },
    title: {
      text: selectedItem.value,
      style: {
        fontSize: "14px",
        fontWeight: "bold",
        color: "#e4e4e7",
      },
      rotate: -90,
      // offsetX: -8,
      offsetY: -10,
    },
  },
  yaxis: {
    show: false,
  },
});
let chart = null;

watchEffect(async () => {
  getChartOptions.value.xaxis.title.text = selectedLabel.value;
  getChartOptions.value.series[0].color =
    rate.value >= 0 ? "#1C64F2" : "#E02424";
  getChartOptions.value.fill.gradient.shade =
    rate.value >= 0 ? "#1C64F2" : "#FBD5D5";
  getChartOptions.value.fill.gradient.gradientToColors =
    rate.value >= 0 ? ["#1C64F2"] : ["#FBD5D5"];
});

watch(
  () => selectedItem.value,
  async (newValue) => {
    await fetchData();
  },
);

onMounted(async () => {
  await fetchData();
  chart = new ApexCharts(chartRef.value, getChartOptions.value);
  chart.render();
});

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;

    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/gettotalpages?period=${selectedItem.value}`,
    );
    const result = await response.json();

    // Update reactive data
    data.value = result.lastPagesCreated;
    categories.value = result.lastPagesCreatedDates;
    rate.value = result.rate;
    value.value = result.totalPages;

    getChartOptions.value.series[0].data = data.value;
    getChartOptions.value.xaxis.categories = categories.value;
    if (chart) {
      chart.updateOptions(getChartOptions.value);
    }
    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};
</script>
