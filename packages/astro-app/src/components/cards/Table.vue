<template>
  <div
    class="tw-relative tw-col-span-3 tw-h-full tw-flex tw-items-center tw-w-full tw-bg-white tw-rounded-lg tw-shadow tw-px-1 tw-py-2"
  >
    <div
      v-if="!loading"
      class="tw-w-full tw-flex tw-flex-col tw-justify-between tw-h-full tw-relative tw-overflow-hidden"
    >
      <table class="tw-w-full">
        <thead
          class="tw-text-xs tw-text-gray-700 tw-uppercase tw-bg-gray-50 tw-text-left"
        >
          <template
            v-for="(headerGroup, index) in table.getHeaderGroups()"
            :key="headerGroup.id"
          >
            <tr v-if="index === 1">
              <template
                v-for="(header, headerIndex) in headerGroup.headers"
                :key="header.id"
              >
                <th
                  v-if="header.column.id !== 'email' && headerIndex !== 0"
                  :colSpan="header.colSpan"
                  class="tw-px-3 tw-py-1.5"
                >
                  <FlexRender
                    :render="header.column.columnDef.header"
                    :props="header.getContext()"
                  />
                </th>
              </template>
            </tr>
          </template>
        </thead>
        <tbody>
          <tr
            class="tw-bg-white tw-border-b dark:tw-bg-gray-800 dark:tw-border-gray-700 hover:tw-bg-gray-50 dark:hover:tw-bg-gray-600"
            v-for="row in table.getRowModel().rows"
            :key="row.id"
          >
            <template
              v-for="(cell, index) in row.getVisibleCells()"
              :key="cell.id"
            >
              <td v-if="cell.column.id !== 'email' && index !== 0">
                <div v-if="index === 4" class="tw-flex tw-items-center">
                  <div
                    class="tw-h-2.5 tw-w-2.5 tw-rounded-full tw-bg-green-500 tw-me-2"
                  ></div>
                  <FlexRender
                    :render="cell.column.columnDef.cell"
                    :props="cell.getContext()"
                  />
                </div>
                <div
                  v-else
                  class="tw-flex tw-items-center tw-px-1.5 tw-py-1 tw-text-gray-900 tw-whitespace-nowrap dark:tw-text-white"
                >
                  <template v-if="index === 1">
                    <div
                      class="tw-relative tw-inline-flex tw-items-center tw-justify-center tw-w-10 tw-h-10 tw-overflow-hidden tw-bg-accent tw-rounded-full dark:tw-bg-gray-600"
                    >
                      <span
                        class="tw-font-medium tw-text-gray-600 dark:tw-text-gray-300"
                        >{{
  cell.getValue('name').split(" ").length > 1
  ? cell.getValue('name').split(" ")[0].charAt(0) + cell.getValue('name').split(" ").slice(-1)[0].charAt(0)
  : cell.getValue('name').substring(0, 2)
}}
</span
                      >
                    </div>
                    <div class="tw-ps-3">
                      <div class="tw-text-base tw-font-semibold">
                        <FlexRender
                          :render="cell.column.columnDef.cell"
                          :props="cell.getContext()"
                        />
                      </div>
                      <div class="tw-font-normal tw-text-gray-500">
                        <FlexRender
                          :render="
                            row.getVisibleCells()[index + 1].column.columnDef
                              .cell
                          "
                          :props="row.getVisibleCells()[index + 1].getContext()"
                        />
                      </div>
                    </div>
                  </template>
                  <template v-else>
                    <div class="tw-ps-3">
                      <FlexRender
                        :render="cell.column.columnDef.cell"
                        :props="cell.getContext()"
                      />
                    </div>
                  </template>
                </div>
              </td>
            </template>
          </tr>
        </tbody>
      </table>

      <nav
        class="tw-flex tw-justify-center tw-items-center tw-h-8 tw-relative"
        aria-label="Table navigation"
      >
        <ul
          class="tw-inline-flex -tw-space-x-px rtl:tw-space-x-reverse tw-text-sm tw-h-6 tw-my-auto"
        >
          <li>
            <a
              @click="
                () =>
                  (!table.getCanPreviousPage() ? null : table.previousPage())
              "
              :class="
                table.getCanPreviousPage()
                  ? 'tw-cursor-pointer'
                  : 'tw-cursor-not-allowed'
              "
              class="tw-flex tw-items-center tw-justify-center tw-px-3 tw-h-6 tw-ms-0 tw-leading-tight tw-text-gray-500 tw-bg-white tw-border tw-border-gray-300 hover:tw-bg-gray-100 hover:tw-text-gray-700 dark:tw-bg-gray-800 dark:tw-border-gray-700 dark:tw-text-gray-400 dark:hover:tw-bg-gray-700 dark:hover:tw-text-white"
              >«</a
            >
          </li>
          <li>
            <a
              aria-current="page"
              class="tw-flex tw-items-center tw-justify-center tw-px-3 tw-h-6 tw-border tw-border-gray-300 tw-bg-accent"
              >{{ table.getState().pagination.pageIndex + 1 }}</a
            >
          </li>
          <li>
            <a
              @click="() => (!table.getCanNextPage() ? null : table.nextPage())"
              :class="
                table.getCanNextPage()
                  ? 'tw-cursor-pointer'
                  : 'tw-cursor-not-allowed'
              "
              class="tw-flex tw-items-center tw-justify-center tw-px-3 tw-h-6 tw-leading-tight tw-text-gray-500 tw-bg-white tw-border tw-border-gray-300 hover:tw-bg-gray-100 hover:tw-text-gray-700 dark:tw-bg-gray-800 dark:tw-border-gray-700 dark:tw-text-gray-400 dark:hover:tw-bg-gray-700 dark:hover:tw-text-white"
              >»</a
            >
          </li>
        </ul>

        <span class="tw-absolute tw-right-2 tw-text-sm tw-text-gray-700">
          <span class="tw-font-semibold tw-text-gray-900">{{
            table.getPageCount()
          }}</span>
          Page{{ table.getPageCount() > 1 ? "s" : "" }}
        </span>
      </nav>
      <div        data-tg-order="5"
  data-tg-title="Manage and Export Your Members List 🧑‍💼"
            data-tg-tour="View all your members in this detailed table! Easily export the data to Excel or CSV with just one click, making it simple to analyze or share outside the dashboard."
        class="tw-absolute tw-z-10 -tw-bottom-2 tw-left-1 tw-flex tw-justify-end tw-items-center"
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
                /></svg
              ><span class="tw-sr-only">Open dropdown</span>
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
              class="tw-z-30 tw-absolute tw-bottom-0 tw-w-56 tw-origin-bottom-left tw-divide-y tw-divide-gray-100 tw-rounded-md tw-bg-white tw-shadow-lg tw-ring-1 tw-ring-black/5 focus:tw-outline-none"
            >
              <div class="tw-px-1 tw-py-1">
                <MenuItem v-slot="{ active }">
                  <button
                    :class="[
                      active ? 'tw-bg-gray-100 ' : '',
                      'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                    ]"
                    @click="exportToExcel()"
                  >
                    Export to Excel
                  </button>
                </MenuItem>
              </div>
              <div class="tw-px-1 tw-py-1">
                <MenuItem v-slot="{ active }">
                  <button
                    :class="[
                      active ? 'tw-bg-gray-100 ' : '',
                      'tw-text-gray-700 tw-group tw-flex tw-w-full tw-items-center tw-rounded-md tw-px-2 tw-py-2 tw-text-sm',
                    ]"
                     @click="exportToCSV()"
                  >
                    Export to CSV
                  </button>
                </MenuItem>
              </div>
            </MenuItems>
          </transition>
        </Menu>
      </div>

        <div
          class="tw-absolute tw-top-1 tw-right-1 tw-flex tw-justify-end tw-items-center"
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

    </div>
    <div v-else class="tw-flex tw-w-full tw-flex-col tw-gap-2">
      <div class="tw-skeleton tw-h-16 tw-w-full"></div>
      <div class="tw-skeleton tw-h-3 tw-w-28"></div>
      <div class="tw-skeleton tw-h-3 tw-w-full"></div>
    </div>
 <div class="tw-absolute tw-bottom-2 tw-right-40 tw-text-[#e4e4e7] tw-font-bold tw-text-sm">{{selectedLabel}}</div>

  </div>
</template>

<script setup lang="ts">
import { Menu, MenuButton, MenuItem, MenuItems } from "@headlessui/vue";
import {
createColumnHelper,
FlexRender,
getCoreRowModel,
getPaginationRowModel,
useVueTable,
} from "@tanstack/vue-table";
import { computed, onMounted, ref } from "vue";
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import type { MembersList } from "~/utils/interfaces.ts";
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
const INITIAL_PAGE_INDEX = 0;

interface Person {
  selected: boolean;
  id: number;
  name: string;
  group: string;
  lastLogin: string;
}

const defaultData: Person[] = Array.from({ length: 100 }, (_, index) => ({
  selected: false,
  name: `Name ${index}`,
  group: `Group ${index}`,
  lastLogin: `Logged in ${index}`,
}));

const columnHelper = createColumnHelper<Person>();
const goToPageNumber = ref(INITIAL_PAGE_INDEX + 1);
const pageSizes = [4, 10, 20, 30, 40, 50];
const data = ref([]);

const columns = [
  columnHelper.group({
    header: "Name",
    columns: [
      columnHelper.accessor((row) => row.selected, {
        id: "selected",
        cell: (info) => info.getValue(),
        header: () => "",
      }),
      columnHelper.accessor("name", {
        cell: (info) => info.getValue(),
        header: () => "Name",
      }),
      columnHelper.accessor("email", {
        id: "email",
        cell: (info) => info.getValue(),
        header: () => "Email",
      }),
      columnHelper.accessor((row) => row.group, {
        id: "group",
        cell: (info) => info.getValue(),
        header: () => "Group",
      }),
      columnHelper.accessor((row) => row.lastLogin, {
        id: "lastLogin",
        cell: (info) => info.getValue(),
        header: () => "Last Login",
      }),
    ],
  }),
];

const table = ref(null);

function rerender() {
  data.value = [...defaultData];
}

function handleGoToPage(e: any) {
  const page = e.target.value ? Number(e.target.value) - 1 : 0;
  goToPageNumber.value = page + 1;
  table.value.setPageIndex(page);
}

function handlePageSizeChange(e: any) {
  table.value.setPageSize(Number(e.target.value));
}

onMounted(async () => {
  await fetchData();
  incrementCardsLoadedCount();
});

const fetchData = async () => {
  try {
    const API_PREFIX = import.meta.env.PUBLIC_API_PREFIX;

    const response = await fetch(
      `${props.currentHost}${API_PREFIX}api/astroboard/getmembers?period=${selectedItem.value}`,
    );
    const result: MembersList = await response.json();
    data.value = result.members;
    // Update reactive data
    table.value = useVueTable({
      data: data.value,
      columns,
      getCoreRowModel: getCoreRowModel(),
      getPaginationRowModel: getPaginationRowModel(),
      initialState: {
        pagination: {
          pageIndex: 0, //custom initial page index
          pageSize: 4, //custom default page size
        },
      },
    });
    loading.value = false;
  } catch (error) {
    console.error("Error fetching data:", error);
  }
};

const exportToExcel = async () => {
  try {
    // Create workbook
    const workbook = new ExcelJS.Workbook();
    workbook.creator = 'Umbraco Astroboard';
    workbook.created = new Date();

    // Add worksheet
    const worksheet = workbook.addWorksheet('Members');

    // Add headers
    worksheet.columns = [
      { header: 'Name', key: 'name', width: 25 },
      { header: 'Email', key: 'email', width: 30 },
      { header: 'Group', key: 'group', width: 20 },
      { header: 'Last Login', key: 'lastLogin', width: 20 }
    ];

    // Style headers
    worksheet.getRow(1).font = { bold: true };
    worksheet.getRow(1).fill = {
      type: 'pattern',
      pattern: 'solid',
      fgColor: { argb: 'FFD3D3D3' }
    };

    // Add data rows
    data.value.forEach(member => {
      worksheet.addRow({
        name: member.name,
        email: member.email,
        group: member.group,
        lastLogin: member.lastLogin
      });
    });

    // Generate buffer and download
    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(
      new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' }),
      `astroboard_members_${new Date().toISOString().slice(0,10)}.xlsx`
    );
  } catch (error) {
    console.error('Error exporting to Excel:', error);
    alert('Failed to export to Excel. Please try again.');
  }
};

const exportToCSV = () => {
  try {
    // Create CSV content
    const headers = ['Name', 'Email', 'Group', 'Last Login'].join(',');
    const rows = data.value.map(item =>
      `"${item.name?.replace(/"/g, '""')}","${item.email?.replace(/"/g, '""')}","${item.group?.replace(/"/g, '""')}","${item.lastLogin?.replace(/"/g, '""')}"`
    ).join('\n');

    // Create and download file
    const csv = `${headers}\n${rows}`;
    saveAs(
      new Blob([csv], { type: 'text/csv;charset=utf-8;' }),
      `astroboard_members_${new Date().toISOString().slice(0,10)}.csv`
    );
  } catch (error) {
    console.error('Error exporting to CSV:', error);
    alert('Failed to export to CSV. Please try again.');
  }
};
</script>
