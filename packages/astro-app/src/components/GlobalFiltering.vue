<template>
  <div class="tw-col-span-4">
    <!-- Headline -->
    <h1 class="tw-text-2xl tw-font-bold tw-mb-3">Your overview</h1>

    <div class="tw-w-full">
      <!-- Headless UI Dropdown -->
      <Menu as="div" class="tw-relative tw-inline-block tw-text-left tw-mb-3">
        <div>
          <MenuButton
            class="tw-inline-flex tw-items-center tw-justify-center tw-gap-2 tw-rounded-md tw-border tw-border-gray-300 tw-bg-white tw-px-3 tw-py-2 tw-text-sm tw-font-medium tw-text-gray-700 tw-shadow-sm hover:tw-bg-gray-50 focus:tw-outline-none focus:tw-ring-2 focus:tw-ring-blue-500"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              class="tw-h-4 tw-w-4"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"
              />
            </svg>
            Filters
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
            class="tw-absolute tw-left-0 tw-z-10 tw-mt-2 tw-w-56 tw-origin-top-left tw-divide-y tw-divide-gray-100 tw-rounded-md tw-bg-white tw-shadow-lg tw-ring-1 tw-ring-black tw-ring-opacity-5 focus:tw-outline-none"
          >
            <div class="tw-py-1">
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('today')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  Today
                </button>
              </MenuItem>
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('yesterday')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  Yesterday
                </button>
              </MenuItem>
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('currentweek')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  This Week
                </button>
              </MenuItem>
            </div>
            <div class="tw-py-1">
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('lastweek')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  Last 7 days
                </button>
              </MenuItem>
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('lastmonth')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  Last 30 days
                </button>
              </MenuItem>
            </div>
            <div class="tw-py-1">
              <MenuItem v-slot="{ active }">
                <button
                  @click="selectFilter('last90days')"
                  :class="[
                    active
                      ? 'tw-bg-gray-100 tw-text-gray-900'
                      : 'tw-text-gray-700',
                    'tw-block tw-w-full tw-px-4 tw-py-2 tw-text-left tw-text-sm',
                  ]"
                >
                  Last 90 days
                </button>
              </MenuItem>
            </div>
          </MenuItems>
        </transition>
      </Menu>

      <!-- Horizontal Line Separator -->
      <div class="tw-border-t tw-border-gray-200 tw-my-1"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Menu, MenuButton, MenuItems, MenuItem } from "@headlessui/vue";

const emit = defineEmits(["filterSelected"]);

const selectFilter = (period: string) => {
  emit("filterSelected", period);
};
</script>
