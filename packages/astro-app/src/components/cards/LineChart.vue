<template>
  <div
    v-if="!loading"
    class="tw-col-span-2 tw-h-fit tw-w-full tw-bg-white tw-rounded-lg tw-shadow dark:tw-bg-gray-800 tw-p-2"
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
        <!-- <Menu as="div" class="tw-relative tw-inline-block tw-text-left">
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
        </Menu> -->
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

    <div class="tw-mt-6" ref="chartRef"></div>
  </div>
  <div
    v-else
    class="tw-flex tw-w-full tw-flex-col tw-gap-2 tw-bg-white tw-p-2 tw-rounded-lg"
  >
    <div class="tw-skeleton tw-h-32 tw-w-full"></div>
    <div class="tw-skeleton tw-h-3 tw-w-full"></div>
    <div class="tw-skeleton tw-h-10 tw-w-full tw-rounded-none"></div>
    <div class="tw-skeleton tw-h-3 tw-w-full"></div>
    <div class="tw-skeleton tw-h-10 tw-w-full tw-rounded-none"></div>
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

const selectItem = async (item) => {
  selectedItem.value = item;
  await fetchData();
};

const lineChartData = ref<ContentsLineChartData | null>(null);
let chart = null;

const getChartOptions = ref({
  chart: {
    height: "55%",
    // maxWidth: "100%",
    type: "line",
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
      position: "topRight",
      offsetX: 0,
      offsetY: -120,
    },
    enabled: true,
    x: {
      show: false,
    },
  },
  dataLabels: {
    enabled: false,
  },
  grid: {
    show: true,
    strokeDashArray: 4,
    padding: {
      left: 2,
      right: 2,
      top: -26,
    },
  },
  series: lineChartData.value?.series,
  // [
  //   {
  //     name: "Published",
  //     color: "#2bc37c",
  //     data: [
  //       { x: "Mon", y: 231 },
  //       { x: "Tue", y: 122 },
  //       { x: "Wed", y: 63 },
  //       { x: "Thu", y: 421 },
  //       { x: "Fri", y: 122 },
  //       { x: "Sat", y: 323 },
  //       { x: "Sun", y: 111 },
  //     ],
  //   },
  //   {
  //     name: "Draft",
  //     color: "#3544b1",
  //     data: [
  //       { x: "Mon", y: 50 },
  //       { x: "Tue", y: 50 },
  //       { x: "Wed", y: 50 },
  //       { x: "Thu", y: 50 },
  //       { x: "Fri", y: 50 },
  //       { x: "Sat", y: 50 },
  //       { x: "Sun", y: 50 },
  //     ],
  //   },
  //   {
  //     name: "UnPublished",
  //     color: "#ff9412",
  //     data: [
  //       { x: "Mon", y: 30 },
  //       { x: "Tue", y: 50 },
  //       { x: "Wed", y: 50 },
  //       { x: "Thu", y: 50 },
  //       { x: "Fri", y: 122 },
  //       { x: "Sat", y: 2 },
  //       { x: "Sun", y: 10 },
  //     ],
  //   },
  //   {
  //     name: "Trashed",
  //     color: "#d42054",
  //     data: [
  //       { x: "Mon", y: 232 },
  //       { x: "Tue", y: 113 },
  //       { x: "Wed", y: 341 },
  //       { x: "Thu", y: 224 },
  //       { x: "Fri", y: 522 },
  //       { x: "Sat", y: 411 },
  //       { x: "Sun", y: 243 },
  //     ],
  //   },
  // ]
  legend: {
    show: false,
  },
  stroke: {
    curve: "smooth",
    width: 6,
  },
  xaxis: {
    categories: lineChartData.value?.categories,
    // categories: [
    //   "01 Feb",
    //   "02 Feb",
    //   "03 Feb",
    //   "04 Feb",
    //   "05 Feb",
    //   "06 Feb",
    //   "07 Feb",
    // ]
    labels: {
      show: true,
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
  },
  yaxis: {
    // show: false,
  },
});

onMounted(async () => {
  await fetchData();
  chart = new ApexCharts(chartRef.value, getChartOptions.value);
  chart.render();
});

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;

    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getcontentslinechartdata?period=${selectedItem.value}`,
    );
    lineChartData.value = await response.json();

    getChartOptions.value.series = lineChartData.value?.series;
    getChartOptions.value.xaxis.categories = lineChartData.value?.categories;
    loading.value = false;
    if (chart) {
      chart.updateOptions(getChartOptions.value);
    }
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};
</script>
