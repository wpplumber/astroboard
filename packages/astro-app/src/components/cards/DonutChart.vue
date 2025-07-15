<template>
  <div
    class="tw-h-full tw-flex tw-items-center tw-w-full tw-bg-white tw-rounded-lg tw-shadow tw-px-1 tw-py-2 tw-max-h-[23vh]"
  >
    <div
      v-if="!loading"
      class="tw-h-full tw-w-full tw-px-1 tw-relative tw-flex tw-flex-col"
    >
      <!-- Chart Container -->
      <div
        data-tg-order="3"
        data-tg-title="Track Your Content Status 📊"
        data-tg-tour="Easily monitor your pages with this donut chart! Hover over the legend to highlight Published, Unpublished, or Deleted pages and see a clear breakdown of your content's current status."
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
      class="tw-relative tw-w-full tw-h-full tw-flex tw-flex-col tw-justify-end tw-bg-white tw-rounded-lg tw-animate-pulse"
    >
      <!-- Chart Area Skeleton -->
      <div
        class="tw-absolute tw-inset-0 tw-flex tw-items-center tw-justify-center"
      >
        <div
          class="tw-w-32 tw-h-32 tw-rounded-full tw-bg-gray-200 tw-border-8 tw-border-gray-100"
        ></div>
      </div>

      <!-- Legend Skeleton -->
      <div
        class="tw-absolute tw-right-4 tw-top-1/2 tw-transform tw--translate-y-1/2 tw-space-y-3"
      >
        <div v-for="i in 3" :key="i" class="tw-flex tw-items-center tw-gap-2">
          <div class="tw-w-3 tw-h-3 tw-rounded-full tw-bg-gray-200"></div>
          <div class="tw-h-3 tw-w-16 tw-bg-gray-200 tw-rounded"></div>
        </div>
      </div>

      <!-- Filter Button Skeleton -->
      <div
        class="tw-absolute tw-top-0 tw-right-2 tw-w-8 tw-h-8 tw-bg-gray-200 tw-rounded-lg"
      ></div>

      <!-- Period Label Skeleton -->
      <div
        class="tw-absolute tw-bottom-2 tw-right-10 tw-h-4 tw-w-24 tw-bg-gray-200 tw-rounded"
      ></div>

      <!-- Title Skeleton -->
      <div
        class="tw-absolute tw-top-0 tw-left-2 tw-h-5 tw-w-32 tw-bg-gray-200 tw-rounded"
      ></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import * as echarts from "echarts";
import { onMounted, ref, watch, computed, onBeforeUnmount } from "vue";
import type { ECharts, PieSeriesOption } from "echarts";

interface ChartData {
  value: number;
  name: string;
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

const chartData = ref<ChartData[]>([]);
const chartRef = ref<HTMLElement | null>(null);
let chartInstance: ECharts | null = null;

const colorPalette = ["#2bc37c", "#ff9412", "#d42054"];

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value;
};

const handleClickOutside = (event: MouseEvent) => {
  if (!chartRef.value?.contains(event.target as Node)) {
    isMenuOpen.value = false;
  }
};

const selectItem = async (item: string) => {
  selectedItem.value = item;
  isMenuOpen.value = false;
  await fetchData();
};

const initChart = () => {
  if (!chartRef.value) return;

  chartInstance = echarts.init(chartRef.value);
  updateChart();
};

const updateChart = () => {
  if (!chartInstance) return;

  const options: echarts.EChartsOption = {
    graphic: {
      type: "text",
      left: 10,
      top: 15,
      style: {
        text: "Pages stats",
        font: "bold 18px Inter, sans-serif",
        fill: "#111827",
      },
    },
    tooltip: {
      trigger: "item",
      formatter: "{a} <br/>{b}: {c} ({d}%)",
    },
    legend: {
      orient: "vertical",
      right: 5,
      top: "center",
      textStyle: {
        color: "#6b7280",
      },
      data: chartData.value.map((item) => item.name),
    },
    series: [
      {
        name: "Content Status",
        type: "pie",
        radius: ["50%", "70%"],
        avoidLabelOverlap: false,
        padAngle: 5,
        itemStyle: {
          borderRadius: 5,
          borderColor: "#fff",
          borderWidth: 2,
        },
        label: {
          show: true,
          formatter: "{b}: {d}%",
        },
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: "rgba(0, 0, 0, 0.5)",
          },
        },
        data: chartData.value,
        color: colorPalette,
      } as PieSeriesOption,
    ],
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
          name: `Pages_stats_${selectedItem.value}`,
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

  chartInstance.setOption(options);
};

const exportChart = (type: "png" | "svg") => {
  if (!chartInstance) return;
  const url = chartInstance.getDataURL({ type, pixelRatio: 2 });
  const link = document.createElement("a");
  link.href = url;
  link.download = `Pages_stats_${selectedItem.value}.${type}`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const exportData = () => {
  const exportObj = {
    title: "Pages stats",
    data: chartData.value,
    period: selectedItem.value,
  };

  const dataStr = `data:text/json;charset=utf-8,${encodeURIComponent(JSON.stringify(exportObj, null, 2))}`;
  const link = document.createElement("a");
  link.href = dataStr;
  link.download = `Pages_stats_data_${selectedItem.value}.json`;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

const fetchData = async () => {
  try {

    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;
    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getcontentstatususage?period=${selectedItem.value}`,
    );
    const result = await response.json();

    chartData.value = result.labels.map((label: string, index: number) => ({
      name: label,
      value: result.series[index],
    }));

    loading.value = false;
    updateChart();
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
      await fetchData();
    }
  },
);

window.addEventListener("resize", () => {
  if (chartInstance) {
    chartInstance.resize();
  }
});
</script>
