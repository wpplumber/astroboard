<template>
  <div
    class="tw-h-full tw-flex tw-items-center tw-w-full tw-bg-white tw-rounded-lg tw-shadow tw-px-1 tw-py-2 tw-max-h-[23vh]"
  >
    <div
      v-if="!loading"
      class="tw-w-full tw-h-full tw-px-1 tw-relative tw-flex tw-flex-col tw-justify-end tw-bg-white"
    >
      <!-- Chart Container -->
      <div
        data-tg-order="4"
        data-tg-title="Manage Your Asset Types 🗂️"
        data-tg-tour="Visualize your asset stats with this bar chart! Click on any legend—Folder, File, Image, SVG, Audio, or Video—to hide or display it from the chart, customizing your view for a clearer understanding."
        ref="chartRef"
        :style="{ height: '100%', width: '100%' }"
      ></div>

      <!-- Filter Menu Popover -->
      <div
        v-if="isMenuOpen"
        class="tw-absolute tw-z-30 tw-right-0 tw-top-8 tw-w-56 tw-origin-top-right tw-divide-y tw-divide-gray-100 tw-rounded-md tw-bg-white tw-shadow-lg tw-ring-1 tw-ring-black/5 focus:tw-outline-none"
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
      </div>

      <!-- Period Label -->
      <div
        class="tw-absolute tw-bottom-0 tw-right-10 tw-text-[#e4e4e7] tw-font-bold tw-text-sm"
      >
        {{ selectedLabel }}
      </div>
    </div>

    <div
      v-else
      class="tw-h-full tw-flex tw-items-center tw-w-full tw-px-1 tw-py-2 tw-max-h-[23vh]"
    >
      <!-- Loading Skeleton -->
      <div
        class="tw-w-full tw-px-1 tw-relative tw-flex tw-flex-col tw-justify-end tw-h-[20vh]"
      >
        <!-- Title Placeholder (Top Left) -->
        <div class="tw-absolute tw-top-1 tw-left-2">
          <div class="tw-skeleton tw-h-6 tw-w-32 tw-bg-gray-200"></div>
        </div>

        <!-- Menu Button Placeholder (Top Right) -->
        <div class="tw-absolute tw-top-1 tw-right-2">
          <div
            class="tw-skeleton tw-h-6 tw-w-6 tw-rounded tw-bg-gray-200"
          ></div>
        </div>

        <!-- Legend Placeholder (Below Title) -->
        <div
          class="tw-absolute tw-top-10 tw-left-2 tw-right-2 tw-flex tw-flex-wrap tw-gap-2"
        >
          <div v-for="i in 6" :key="i" class="tw-flex tw-items-center tw-gap-1">
            <div
              class="tw-skeleton tw-h-3 tw-w-3 tw-rounded-full tw-bg-gray-200"
            ></div>
            <div class="tw-skeleton tw-h-3 tw-w-12 tw-bg-gray-200"></div>
          </div>
        </div>

        <!-- Horizontal Bar Chart Placeholder -->
        <div
          class="tw-absolute tw-top-20 tw-left-2 tw-right-2 tw-bottom-2 tw-flex tw-flex-col tw-gap-2"
        >
          <div class="tw-w-full tw-grid tw-grid-cols-6 tw-gap-1">
            <div
              v-for="i in 6"
              :key="'bar' + i"
              class="tw-skeleton tw-h-10 tw-w-full tw-rounded tw-bg-gray-200"
            ></div>
          </div>
        </div>

        <!-- Period Label Placeholder (Bottom Right) -->
        <div class="tw-absolute tw-bottom-1 tw-right-2">
          <div
            class="tw-skeleton tw-h-4 tw-w-24 tw-bg-gray-200 tw-rounded"
          ></div>
        </div>
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
  onBeforeUnmount,
  nextTick,
} from "vue";
import type { ECharts } from "echarts";

interface SeriesItem {
  name: string;
  data: { x: string; y: number }[];
}

interface ApiResponse {
  period: string;
  series: SeriesItem[];
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

const loading = ref(true);
const isMenuOpen = ref(false);
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

const chartData = ref<echarts.SeriesOption[]>([]);
const chartRef = ref<HTMLElement | null>(null);
let chartInstance: ECharts | null = null;

const colorPalette = [
  "#2196f3",
  "#9e9e9e",
  "#4caf50",
  "#ffeb3b",
  "#f44336",
  "#9c27b0",
];

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};

const handleClickOutside = (event: MouseEvent) => {
  if (!chartRef.value?.contains(event.target as Node)) {
    isMenuOpen.value = false;
  }
};

const selectItem = async (item: string) => {
  try {
    selectedItem.value = item;
    isMenuOpen.value = false;
    loading.value = true;

    // Clear existing chart
    if (chartInstance) {
      chartInstance.dispose();
      chartInstance = null;
    }

    await fetchData();
    await nextTick(); // Wait for DOM update

    if (chartRef.value) {
      chartInstance = echarts.init(chartRef.value);
      updateChart();
    }
  } catch (error) {
    console.error("Error changing filter:", error);
    loading.value = false;
  }
};

const initChart = () => {
  if (!chartRef.value) return;

  // Clean up any existing instance
  if (chartInstance) {
    chartInstance.dispose();
  }

  chartInstance = echarts.init(chartRef.value);
  updateChart();
};

const updateChart = () => {
  if (!chartInstance) return;

  const options: echarts.EChartsOption = {
    graphic: {
      type: "text",
      left: 5,
      top: 10,
      style: {
        text: "Assets stats",
        font: "bold 18px Inter, sans-serif",
        fill: "#111827",
      },
    },
    tooltip: {
      trigger: "axis",
      axisPointer: {
        type: "shadow",
      },
      formatter: (params: any) => {
        let total = params.reduce(
          (sum: number, param: any) => sum + (param.value || 0),
          0,
        );
        let tooltip = `<div style="font-weight:bold;margin-bottom:5px">${selectedLabel.value}</div>`;

        params.forEach((param: any) => {
          const value = param.value || 0;
          const percent = total > 0 ? ((value / total) * 100).toFixed(1) : "0";
          tooltip += `${param.marker} ${param.seriesName}: <strong>${value}</strong> (${percent}%)<br/>`;
        });

        tooltip += `<hr style="margin:5px 0;border-top:1px solid #eee"/>`;
        tooltip += `Total: <strong>${total}</strong>`;
        return tooltip;
      },
    },
    legend: {
      data: chartData.value.map((item) => item.name) as string[],
      top: 40,
      textStyle: {
        fontSize: 10,
      },
    },
    grid: {
      left: "3%",
      right: "4%",
      bottom: "3%",
      top: "40%",
      containLabel: true,
    },
    xAxis: {
      type: "value",
      show: false,
      axisLabel: { show: false },
      axisLine: { show: false },
      axisTick: { show: false },
    },
    yAxis: {
      type: "category",
      data: [selectedLabel.value],
      show: false,
    },
    series: chartData.value,
    color: colorPalette,
    toolbox: {
      show: true,
      right: 0,
      top: 0,
      feature: {
        myFilterMenu: {
          show: true,
          title: "Filters",
          icon: "path://M4 10.5c-.83 0-1.5-.67-1.5-1.5S3.17 7.5 4 7.5 5.5 8.17 5.5 9 4.83 10.5 4 10.5zm0-6c-.83 0-1.5-.67-1.5-1.5S3.17 1.5 4 1.5 5.5 2.17 5.5 3 4.83 4.5 4 4.5zm0 12c-.83 0-1.5-.67-1.5-1.5s.67-1.5 1.5-1.5 1.5.67 1.5 1.5-.67 1.5-1.5 1.5z",
          onclick: () => toggleMenu(),
        },
        saveAsImage: {
          title: "Save as PNG",
          type: "png",
          pixelRatio: 2,
          name: `Assets_stats_${selectedItem.value}`,
        },
        mySaveAsSVG: {
          show: true,
          title: "Save as SVG",
          icon: "path://M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z",
          onclick: () => exportChart("svg"),
        },
        myExportData: {
          show: true,
          title: "Export Data",
          icon: "path://M17 17h2v2H5v-2h2v-2H5v-2h2v-2H5V9h2V7H5V5h12v12zm-4-6V9h-2v2h2zm0 2h-2v2h2v-2z",
          onclick: () => exportData(),
        },
      },
    },
  };

  chartInstance.setOption(options, true);
};

const exportChart = (type: "png" | "svg") => {
  if (!chartInstance) return;
  const url = chartInstance.getDataURL({ type, pixelRatio: 2 });
  const link = document.createElement("a");
  link.href = url;
  link.download = `Assets_stats_${selectedItem.value}.${type}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const exportData = () => {
  const exportObj = {
    title: "Assets stats",
    data: chartData.value,
    period: selectedItem.value,
  };

  const dataStr = `data:text/json;charset=utf-8,${encodeURIComponent(JSON.stringify(exportObj, null, 2))}`;
  const link = document.createElement("a");
  link.href = dataStr;
  link.download = `Assets_stats_data_${selectedItem.value}.json`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const fetchData = async () => {
  try {

    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;
    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getmedias?period=${selectedItem.value}`,
    );
    const result: ApiResponse = await response.json();

    chartData.value = result.series.map((item, index) => ({
      name: item.name,
      type: "bar",
      stack: "total",
      data: [item.data[0].y],
      itemStyle: {
        color: colorPalette[index % colorPalette.length],
        borderRadius: [0, 4, 4, 0],
      },
      emphasis: {
        focus: "series",
      },
      label: {
        show: false,
      },
    }));

    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

onMounted(async () => {
  await fetchData();
  initChart();
  document.addEventListener("click", handleClickOutside);
});

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.dispose();
    chartInstance = null;
  }
  document.removeEventListener("click", handleClickOutside);
});

watch(
  () => props.globalFilter,
  async (newFilter) => {
    if (newFilter) {
      selectedItem.value = newFilter;
      await fetchData().then(() => {
        if (chartInstance) {
          updateChart();
        } else {
          initChart();
        }
      });
    }
  },
);

window.addEventListener("resize", () => {
  chartInstance?.resize();
});

// Fallback initialization if chart fails to load
watch(loading, (newVal) => {
  if (!newVal && !chartInstance && chartRef.value) {
    initChart();
  }
});
</script>
