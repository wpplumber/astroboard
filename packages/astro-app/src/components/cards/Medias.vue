<template>
  <div
    class="tw-h-full tw-flex tw-items-center tw-max-w-xs tw-w-full tw-bg-white tw-rounded-lg tw-shadow dark:tw-bg-gray-800 px-2 md:tw-px-3"
  >
    <div v-if="!loading" class="tw-w-full tw-h-full tw-relative">
      <div ref="fullChartRef" class="tw-w-full tw-h-full"></div>

      <!-- Headless UI Menu Overlay -->
      <div class="tw-absolute tw-top-0 tw-right-0 tw-z-40" ref="popoverRef">
        <Popover class="tw-relative tw-inline-block tw-text-left">
          <transition
            enter-active-class="tw-transition tw-duration-100 tw-ease-out"
            enter-from-class="tw-transform tw-scale-95 tw-opacity-0"
            enter-to-class="tw-transform tw-scale-100 tw-opacity-100"
            leave-active-class="tw-transition tw-duration-75 tw-ease-in"
            leave-from-class="tw-transform tw-scale-100 tw-opacity-100"
            leave-to-class="tw-transform tw-scale-95 tw-opacity-0"
          >
            <PopoverPanel
              v-if="isMenuOpen"
              static
              class="tw-absolute tw-right-0 tw-mt-2 tw-z-40 tw-w-56 tw-bg-white tw-divide-y tw-divide-gray-100 tw-rounded-md tw-shadow-lg tw-ring-1 tw-ring-black/5 focus:tw-outline-none"
            >
              <div class="tw-px-1 tw-py-1">
                <button
                  @click="selectItem('today')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  Today
                </button>
                <button
                  @click="selectItem('yesterday')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  Yesterday
                </button>
                <button
                  @click="selectItem('currentweek')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  This Week
                </button>
              </div>
              <div class="tw-px-1 tw-py-1">
                <button
                  @click="selectItem('lastweek')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  Last 7 days
                </button>
                <button
                  @click="selectItem('lastmonth')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  Last 30 days
                </button>
              </div>
              <div class="tw-px-1 tw-py-1">
                <button
                  @click="selectItem('last90days')"
                  class="tw-text-sm tw-w-full tw-text-left tw-py-2 tw-px-2 hover:tw-bg-gray-100"
                >
                  Last 90 days
                </button>
              </div>
            </PopoverPanel>
          </transition>
        </Popover>
      </div>
    </div>
    <div v-else class="tw-w-full tw-h-full tw-relative">
      <!-- Title Placeholder (Top Left) -->
      <div class="tw-absolute tw-top-4 tw-left-4">
        <div class="tw-skeleton tw-h-6 tw-w-24 tw-bg-gray-200"></div>
      </div>

      <!-- Value Placeholder (Top Left) -->
      <div class="tw-absolute tw-top-16 tw-left-4">
        <div class="tw-skeleton tw-h-10 tw-w-32 tw-bg-gray-200"></div>
      </div>

      <!-- Zoom Control Placeholder (Bottom) -->
      <div class="tw-absolute tw-bottom-4 tw-left-4 tw-right-4">
        <div
          class="tw-skeleton tw-h-4 tw-w-full tw-bg-gray-200 tw-rounded"
        ></div>
      </div>

      <!-- Filter Menu Button (Top Right) -->
      <div class="tw-absolute tw-top-2 tw-right-2">
        <div class="tw-skeleton tw-h-8 tw-w-8 tw-rounded tw-bg-gray-200"></div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import * as echarts from "echarts";
import {
  onMounted,
  ref,
  watch,
  computed,
  nextTick,
  onBeforeUnmount,
} from "vue";
import { formatValueWithK } from "~/utils/formatting";
import { Popover, PopoverPanel } from "@headlessui/vue";

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

const popoverRef = ref(null);
const loading = ref(true);
const isMenuOpen = ref(false);
const menuButton = ref(null);
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

const title = ref("Media");
const value = ref(0);
const rate = ref(0);
const data = ref<number[]>([]);
const categories = ref<string[]>([]);
const fullChartRef = ref<HTMLElement | null>(null);
let fullChart: echarts.ECharts | null = null;

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;

  if (isMenuOpen.value) {
    // Delay adding outside click listener so it doesn't immediately close
    setTimeout(() => {
      document.addEventListener("click", handleClickOutside);
    }, 0);
  } else {
    document.removeEventListener("click", handleClickOutside);
  }
};

const handleClickOutside = (event) => {
  if (popoverRef.value && !popoverRef.value.contains(event.target)) {
    isMenuOpen.value = false;
    document.removeEventListener("click", handleClickOutside);
  }
};

const selectItem = async (item) => {
  selectedItem.value = item;
  isMenuOpen.value = false;
  await fetchData();
};

const getFullChartOptions = computed(() => {
  const rateColor =
    rate.value >= 0 ? (rate.value === 0 ? "#3b82f6" : "#10b981") : "#ef4444";

  return {
    grid: {
      top: 20,
      left: 80,
      right: 15,
      bottom: 30,
      containLabel: false,
    },
    graphic: [
      {
        type: "text",
        left: 5,
        top: 15,
        style: {
          text: title.value,
          font: "bold 18px Inter, sans-serif",
          fill: "#111827",
        },
      },
      {
        type: "text",
        left: 5,
        top: 60,
        style: {
          text: formatValueWithK(value.value),
          font: "bold 32px Inter, sans-serif",
          fill: "#111827",
        },
      },
      {
        type: "group",
        left: 15,
        top: 110,
        children: [
          {
            type: "text",
            left: -10,
            top: 0,
            style: {
              text: rate.value === 0 ? "" : rate.value > 0 ? "↑" : "↓",
              font: "bold 20px Inter, sans-serif",
              fill: rateColor,
            },
          },
          {
            type: "text",
            top: 5,
            style: {
              text: `${rate.value}%`,
              font: "bold 16px Inter, sans-serif",
              fill: rateColor,
            },
          },
        ],
      },
    ],
    xAxis: {
      type: "category",
      data: categories.value,
      axisLine: { show: false },
      axisTick: { show: false },
      axisLabel: { show: false },
      name: selectedLabel.value,
      nameLocation: "middle",
      nameGap: 10,
      nameTextStyle: {
        color: "#e4e4e7",
        fontSize: 14,
        fontWeight: "bold",
      },
    },
    yAxis: {
      type: "value",
      show: false,
    },
    series: [
      {
        data: data.value,
        type: "line",
        smooth: true,
        symbol: "none",
        lineStyle: {
          width: 4,
          color: rate.value >= 0 ? "#1A56DB" : "#E02424",
        },
        areaStyle: {
          color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
            {
              offset: 0,
              color:
                rate.value >= 0
                  ? "rgba(28, 100, 242, 0.55)"
                  : "rgba(251, 213, 213, 0.55)",
            },
            {
              offset: 1,
              color:
                rate.value >= 0
                  ? "rgba(28, 100, 242, 0)"
                  : "rgba(251, 213, 213, 0)",
            },
          ]),
        },
      },
    ],
    tooltip: {
      trigger: "axis",
      position: [0, -10],
      formatter: (params: any) => {
        return `${params[0].axisValue}<br/>Media: ${params[0].data}`;
      },
    },
    toolbox: {
      show: true,
      right: 0,
      top: -2,
      feature: {
        // 1. Filter Menu (3-dots)
        myFilterMenu: {
          show: true,
          title: "Filters",
          icon: "path://M4 10.5c-.83 0-1.5-.67-1.5-1.5S3.17 7.5 4 7.5 5.5 8.17 5.5 9 4.83 10.5 4 10.5zm0-6c-.83 0-1.5-.67-1.5-1.5S3.17 1.5 4 1.5 5.5 2.17 5.5 3 4.83 4.5 4 4.5zm0 12c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z",
          onclick: () => toggleMenu(),
        },
        // 2. PNG Export
        saveAsImage: {
          title: "Save as PNG",
          type: "png",
          pixelRatio: 2,
          excludeComponents: ["toolbox"],
          name: `${title.value}_${selectedItem.value}`,
        },
        // 3. SVG Export
        mySaveAsSVG: {
          show: true,
          title: "Save as SVG",
          icon: "path://M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z",
          onclick: () => exportChart("svg"),
        },
        // 4. Data Export
        myExportData: {
          show: true,
          title: "Export Data",
          icon: "path://M17 17h2v2H5v-2h2v-2H5v-2h2v-2H5V9h2V7H5V5h12v12zm-4-6V9h-2v2h2zm0 2h-2v2h2v-2z",
          onclick: () => exportData(),
        },
      },
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
});

const exportChart = (type: "png" | "svg") => {
  if (!fullChart) return;
  const url = fullChart.getDataURL({ type, pixelRatio: 2 });
  const link = document.createElement("a");
  link.href = url;
  link.download = `${title.value}-${selectedItem.value}.${type}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const exportData = () => {
  const exportObj = {
    title: title.value,
    data: data.value,
    categories: categories.value,
    period: selectedItem.value,
  };

  const dataStr = `data:text/json;charset=utf-8,${encodeURIComponent(JSON.stringify(exportObj, null, 2))}`;
  const link = document.createElement("a");
  link.href = dataStr;
  link.download = `${title.value}-data.json`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;
    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/gettotalmedias?period=${selectedItem.value}`,
    );
    const result = await response.json();

    data.value = result.lastMediasCreated;
    categories.value = result.lastMediasCreatedDates;
    rate.value = result.rate;
    value.value = result.totalMedias;

    if (fullChart) {
      fullChart.setOption(getFullChartOptions.value);
    }
    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

onMounted(async () => {
  await fetchData();
  await nextTick();

  if (fullChartRef.value) {
    fullChart = echarts.init(fullChartRef.value);
    fullChart.setOption(getFullChartOptions.value);

    const resizeObserver = new ResizeObserver(() => {
      fullChart?.resize();
    });
    resizeObserver.observe(fullChartRef.value);

    setTimeout(() => {
      fullChart?.resize();
    }, 100);
  }
});

onBeforeUnmount(() => {
  if (fullChart) {
    fullChart.dispose();
  }
});

watch(
  () => props.globalFilter,
  async (newFilter) => {
    if (newFilter) {
      selectedItem.value = newFilter;
      await fetchData();
    }
  },
);
</script>
