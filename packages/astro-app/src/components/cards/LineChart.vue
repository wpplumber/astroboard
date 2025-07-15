<template>
  <div
    class="tw-relative tw-col-span-2 tw-h-full tw-flex tw-items-center tw-w-full tw-bg-white tw-rounded-lg tw-shadow tw-px-1 tw-py-2"
  >
    <div
      v-if="!loading"
      class="tw-col-span-2 tw-h-fit tw-w-full tw-bg-white dark:tw-bg-gray-800 tw-p-2"
    >
      <div
        class="tw-flex tw-justify-between tw-pb-4 tw-mb-4 tw-border-b tw-border-gray-200 dark:tw-border-gray-700"
      >
        <div class="tw-flex tw-items-center">
          <div
            class="tw-w-12 tw-h-12 tw-rounded-lg tw-bg-gray-100 dark:tw-bg-gray-700 tw-flex tw-items-center tw-justify-center tw-me-3"
          >
            <i-tabler:versions-filled
              class="tw-w-6 tw-h-6 tw-text-blue-500 dark:tw-text-gray-400"
            />
          </div>
          <div>
            <h5
              class="tw-leading-none tw-text-2xl tw-font-bold tw-text-gray-900 dark:tw-text-white tw-pb-1"
            >
              {{ lineChartData?.pagesPerPeriod }}
            </h5>
            <p
              class="tw-text-sm tw-font-normal tw-text-gray-500 dark:tw-text-gray-400"
            >
              Pages generated {{ selectedLabel }}
            </p>
          </div>
        </div>
        <div class="tw-flex tw-flex-col tw-text-center tw-gap-y-1">
          <span
            :class="[
              lineChartData?.rate > 0
                ? 'tw-bg-green-100 tw-text-green-800'
                : lineChartData?.rate === 0
                  ? 'tw-bg-blue-100 tw-text-blue-800'
                  : 'tw-bg-red-100 tw-text-red-800',
              'tw-text-xs tw-font-medium tw-inline-flex tw-items-center tw-px-2.5 tw-py-1 tw-rounded-md dark:tw-bg-green-900 dark:tw-text-green-300',
            ]"
          >
            <svg
              v-if="lineChartData?.rate !== 0"
              :class="[
                lineChartData?.rate >= 0 ? '' : 'tw-rotate-180',
                'tw-w-2.5 tw-h-2.5 tw-me-1.5',
              ]"
              aria-hidden="true"
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 10 14"
            >
              <path
                stroke="currentColor"
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M5 13V1m0 0L1 5m4-4 4 4"
              />
            </svg>
            {{ lineChartData?.rate }}%
          </span>
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
      </div>

      <div class="tw-grid tw-grid-cols-2">
        <dl class="tw-flex tw-items-center">
          <dt
            class="tw-text-gray-500 dark:tw-text-gray-400 tw-text-sm tw-font-normal tw-me-1"
          >
            Total Contributors:
          </dt>
          <dd
            class="tw-text-gray-900 tw-text-sm dark:tw-text-white tw-font-semibold"
          >
            {{ lineChartData?.contributors }}
          </dd>
        </dl>
        <dl class="tw-flex tw-items-center tw-justify-end">
          <dt
            class="tw-text-gray-500 dark:tw-text-gray-400 tw-text-sm tw-font-normal me-1"
          >
            Daily change rate:
          </dt>
          <dd
            class="tw-text-gray-900 tw-text-sm dark:tw-text-white tw-font-semibold"
          >
            {{ lineChartData?.dailyChangeRate }}%
          </dd>
        </dl>
      </div>

      <div class="tw-mt-6" ref="chartRef" style="height: 200px"></div>
    </div>
    <div v-else class="tw-space-y-4 tw-w-full">
      <!-- Header Skeleton -->
      <div class="tw-flex tw-justify-between">
        <div class="tw-flex tw-items-center tw-space-x-3">
          <div
            class="tw-skeleton tw-w-12 tw-h-12 tw-rounded-lg tw-bg-gray-200"
          ></div>
          <div class="tw-space-y-2">
            <div class="tw-skeleton tw-h-6 tw-w-32 tw-bg-gray-200"></div>
            <div class="tw-skeleton tw-h-4 tw-w-48 tw-bg-gray-200"></div>
          </div>
        </div>
        <div class="tw-flex tw-flex-col tw-items-end tw-space-y-2">
          <div class="tw-skeleton tw-h-6 tw-w-20 tw-bg-gray-200"></div>
          <div
            class="tw-skeleton tw-h-8 tw-w-8 tw-rounded-lg tw-bg-gray-200"
          ></div>
        </div>
      </div>

      <!-- Stats Skeleton -->
      <div class="tw-grid tw-grid-cols-2 tw-gap-4">
        <div class="tw-skeleton tw-h-5 tw-w-full tw-bg-gray-200"></div>
        <div class="tw-skeleton tw-h-5 tw-w-full tw-bg-gray-200"></div>
      </div>

      <!-- Chart Area Skeleton -->
      <div class="tw-space-y-3">
        <div class="tw-skeleton tw-h-5 tw-w-full tw-bg-gray-200"></div>
        <div
          class="tw-skeleton tw-h-40 tw-w-full tw-rounded-lg tw-bg-gray-200"
        ></div>
        <div class="tw-flex tw-justify-between">
          <div class="tw-skeleton tw-h-4 tw-w-16 tw-bg-gray-200"></div>
          <div class="tw-skeleton tw-h-4 tw-w-16 tw-bg-gray-200"></div>
          <div class="tw-skeleton tw-h-4 tw-w-16 tw-bg-gray-200"></div>
          <div class="tw-skeleton tw-h-4 tw-w-16 tw-bg-gray-200"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Menu, MenuButton, MenuItem, MenuItems } from "@headlessui/vue";
import * as echarts from "echarts";
import { onMounted, ref, watch, computed, onBeforeUnmount } from "vue";
import type { ECharts } from "echarts";

interface SeriesItem {
  name: string;
  color: string;
  data: { x: string; y: number }[];
}

interface ContentsLineChartData {
  pagesPerPeriod: number;
  rate: number;
  contributors: number;
  dailyChangeRate: number;
  series: SeriesItem[];
  categories: string[];
}

const props = defineProps({
  currentHost: {
    type: String,
    required: true,
  },
  globalFilter: {
    type: String,
    default: null,
  },
});

const alertMessage = ref(`Enter license to unlock all features!`);
const emit = defineEmits();

function showAlert() {
  emit("showAlert", { msg: alertMessage.value, type: "alert" });
}

const chartRef = ref<HTMLElement | null>(null);
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

const lineChartData = ref<ContentsLineChartData | null>(null);
let chartInstance: ECharts | null = null;

const selectItem = async (item: string) => {
  selectedItem.value = item;
  await fetchData();
  initChart();
};

const initChart = () => {
  if (!chartRef.value) return;

  // Dispose existing chart if any
  if (chartInstance) {
    chartInstance.dispose();
  }

  chartInstance = echarts.init(chartRef.value);
  updateChart();
};

const updateChart = () => {
  if (!chartInstance || !lineChartData.value) return;

  const options = {
    tooltip: {
      trigger: "axis",
      position: function (point: number[]) {
        return [point[0], point[1] - 120];
      },
      formatter: (params: any) => {
        let result = "";
        params.forEach((item: any) => {
          result += `${item.marker} ${item.seriesName}: <b>${item.value[1]}</b><br/>`;
        });
        return result;
      },
    },
    grid: {
      left: "3%",
      right: "4%",
      bottom: "3%",
      top: "5%",
      containLabel: true,
    },
    xAxis: {
      type: "category",
      data: lineChartData.value.categories,
      axisLine: {
        show: false,
      },
      axisTick: {
        show: false,
      },
      axisLabel: {
        color: "#6b7280",
        fontFamily: "Inter, sans-serif",
        fontSize: 12,
      },
    },
    yAxis: {
      type: "value",
      splitLine: {
        lineStyle: {
          type: "dashed",
        },
      },
    },
    series: lineChartData.value.series.map((series) => ({
      name: series.name,
      type: "line",
      smooth: true,
      symbol: "circle",
      symbolSize: 6,
      lineStyle: {
        width: 3,
        color: series.color,
      },
      itemStyle: {
        color: series.color,
      },
      data: series.data.map((item) => [item.x, item.y]),
    })),
    legend: {
      show: false,
    },
    // Draggable Zoom Configuration
    dataZoom: [
      {
        type: "inside",
        xAxisIndex: 0,
        filterMode: "filter",
        zoomLock: false,
        moveOnMouseMove: true,
        preventDefaultMouseMove: true,
      },
      {
        type: "slider",
        xAxisIndex: 0,
        filterMode: "filter",
        height: 15,
        bottom: 10,
        start: 0,
        end: 100,
        handleIcon:
          "M10.7,11.9v-1.3H9.3v1.3c-4.9,0.3-8.8,4.4-8.8,9.4c0,5,3.9,9.1,8.8,9.4v1.3h1.3v-1.3c4.9-0.3,8.8-4.4,8.8-9.4C19.5,16.3,15.6,12.2,10.7,11.9z M13.3,24.4H6.7V23h6.6V24.4z M13.3,19.6H6.7v-1.4h6.6V19.6z",
        handleSize: "80%",
        handleStyle: {
          color: "#fff",
          shadowBlur: 3,
          shadowColor: "rgba(0, 0, 0, 0.6)",
          shadowOffsetX: 2,
          shadowOffsetY: 2,
        },
      },
    ],
    animation: true,
    animationDuration: 1000,
    animationEasing: "cubicInOut",
  };

  chartInstance.setOption(options);
};

const fetchData = async () => {
  try {
    loading.value = true;
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;
    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getcontentslinechartdata?period=${selectedItem.value}`,
    );
    lineChartData.value = await response.json();

    if (chartInstance) {
      updateChart();
    } else {
      initChart();
    }

    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
    loading.value = false;
  }
};

onMounted(async () => {
  await fetchData();
  initChart();
});

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.dispose();
  }
});

watch(
  () => props.globalFilter,
  async (newFilter) => {
    if (newFilter) {
      selectedItem.value = newFilter;
      await fetchData();
      initChart();
    }
  },
);

// Handle window resize
window.addEventListener("resize", () => {
  chartInstance?.resize();
});
</script>
