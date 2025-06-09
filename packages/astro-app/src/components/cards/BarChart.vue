<template>
  <div
    class="tw-h-full tw-flex tw-items-center tw-w-full tw-bg-white tw-rounded-lg tw-shadow tw-px-1 tw-py-2"
  >
    <div
      v-if="!loading"
      class="tw-w-full tw-px-1 tw-relative tw-flex tw-flex-col tw-justify-end tw-bg-white tw-rounded-lg tw-h-[20vh]"
    >
      <div
        class="tw-absolute tw-z-10 -tw-top-2 tw-right-1 tw-flex tw-justify-end tw-items-center"
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

      <div
        data-tg-order="4"
        data-tg-title="Manage Your Asset Types 🗂️"
        data-tg-tour="Visualize your asset stats with this bar chart! Click on any legend—Folder, File, Image, SVG, Audio, or Video—to hide or display it from the chart, customizing your view for a clearer understanding."
        class="pt-2"
        ref="chartRef"
      ></div>
      <div
        class="tw-absolute -tw-bottom-1 tw-right-10 tw-text-[#e4e4e7] tw-font-bold tw-text-sm"
      >
        {{ selectedLabel }}
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
import ApexCharts from "apexcharts";
import { computed, onMounted, ref } from "vue";

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

const chart = ref(null);
const chartRef = ref(null);
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
const series = ref([]);
const selectItem = async (item) => {
  selectedItem.value = item;
  await fetchData();
};

const getChartOptions = ref({
  colors: ["#2196f3", "#9e9e9e", "#4caf50", "#ffeb3b", "#f44336", "#9c27b0"],
  series: series.value,
  chart: {
    type: "bar",
    // height: 150,
    height: "100%",
    // width: "100%",
    fontFamily: "Inter, sans-serif",
    toolbar: {
      show: false,
    },
    stacked: true,
    stackType: "100%",
  },
  title: {
    text: "Assets stats",
    align: "left",
    style: {
      fontSize: "18px",
      fontWeight: "bold",
      fontFamily: "inherit",
      color: "#111827",
    },
  },
  plotOptions: {
    bar: {
      horizontal: true,
      // columnWidth: "70%",
      // borderRadiusApplication: "end",
      // borderRadius: 8,
    },
  },
  tooltip: {
    shared: true,
    intersect: false,
    style: {
      fontFamily: "Inter, sans-serif",
    },
  },
  states: {
    hover: {
      filter: {
        type: "darken",
        value: 1,
      },
    },
  },
  stroke: {
    show: true,
    width: 0,
    colors: ["transparent"],
  },
  grid: {
    show: false,
    strokeDashArray: 4,
    padding: {
      left: 2,
      right: 2,
      top: -14,
    },
  },
  dataLabels: {
    enabled: false,
  },
  legend: {
    show: true,
    fontSize: "7px",
    position: "top",
    // verticalAlign: "bottom"
  },
  xaxis: {
    labels: {
      show: false,
      style: {
        fontFamily: "Inter, sans-serif",
        cssClass: "tw-text-xs tw-font-normal tw-fill-gray-500",
      },
    },
    axisBorder: {
      show: false,
    },
    axisTicks: {
      show: false,
    },
    categories: [selectedLabel.value],
  },
  yaxis: {
    show: false,
    // title: {
    //       text: selectedItem.value,
    //       offsetX: 0,
    //       offsetY: 0,
    //       style: {
    //           color: undefined,
    //           fontSize: '12px',
    //           fontFamily: 'Helvetica, Arial, sans-serif',
    //           fontWeight: 600,
    //           cssClass: 'apexcharts-xaxis-title',
    //       },
    //   },
  },
  fill: {
    opacity: 1,
  },
});

onMounted(async () => {
  await fetchData();
  chart.value = new ApexCharts(chartRef.value, getChartOptions.value);
  chart.value.render();
});

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;

    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getmedias?period=${selectedItem.value}`,
    );
    const result: ContentsBarChartData = await response.json();
    series.value = result.series;
    getChartOptions.value.series = series.value;
    getChartOptions.value.xaxis.categories[0] = selectedLabel.value;

    if (chart.value) {
      chart.value.updateOptions(getChartOptions.value);
    }
    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};
</script>
