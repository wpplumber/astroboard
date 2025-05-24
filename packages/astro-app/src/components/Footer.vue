<script setup lang="ts">
import {
  Dialog,
  DialogPanel,
  DialogTitle,
  TransitionChild,
  TransitionRoot,
} from "@headlessui/vue";
import { XMarkIcon } from "@heroicons/vue/24/outline";
import { ref } from "vue";
import CreateIssue from "./CreateIssue.vue";

const showDrawer = ref(false);

const props = defineProps<{
  computerName: string;
  licensed: boolean;
}>();
</script>

<template>
  <footer class="tw-mt-auto">
    <div
      class="tw-w-full tw-mx-auto tw-max-w-screen-xl tw-p-2 md:tw-flex md:tw-items-center md:tw-justify-center"
    >
      <ul
        class="tw-flex tw-flex-wrap tw-items-center tw-gap-x-2 tw-text-xs tw-font-medium tw-text-gray-500 dark:tw-text-gray-400 sm:tw-mt-0"
      >
        <li>
          <a class="">© 2025 Astroboard</a>
        </li>
        <li>|</li>
        <li>
          <a
            href="https://tarikrital.website/store/astroboard"
            target="_blank"
            class="tw-cursor-pointer tw-hover:tw-underline tw-text-blue-600"
            >Home</a
          >
        </li>
        <li>|</li>
        <li>
          <a
            data-tg-order="7"
            data-tg-title="Manage Your License 🔑"
            data-tg-tour="Access and configure your license settings with ease! Click the link to open the settings dialog and update your license information as needed."
            data-test-id="license"
            class="tw-cursor-pointer tw-hover:tw-underline tw-text-blue-600"
            href="https://astroboard.website"
            >License</a
          >
        </li>
        <li>|</li>
        <li>
          <a
            data-tg-order="8"
            data-tg-title="I Value Your Feedback 💬"
            data-tg-tour="Have suggestions or found an issue? Click here to open the feedback dialog and easily post your thoughts on our GitHub repo. Your input helps us improve!"
            data-test-id="feedback"
            class="tw-cursor-pointer tw-hover:tw-text-gray-800 tw-flex tw-gap-x-1"
            @click="showDrawer = true"
          >
            <i-tabler-message-report class="tw-w-4 tw-h-4" /> Send feedback
          </a>
        </li>
      </ul>
    </div>

    <!-- Drawer for Feedback -->
    <TransitionRoot as="template" :show="showDrawer">
      <Dialog class="tw-relative tw-z-[1111]" @close="showDrawer = false">
        <TransitionChild
          as="template"
          enter="tw-ease-in-out tw-duration-500"
          enter-from="tw-opacity-0"
          enter-to="tw-opacity-100"
          leave="tw-ease-in-out tw-duration-500"
          leave-from="tw-opacity-100"
          leave-to="tw-opacity-0"
        >
          <div
            class="tw-fixed tw-inset-0 tw-bg-gray-500 tw-bg-opacity-75 tw-transition-opacity"
          />
        </TransitionChild>

        <div class="tw-fixed tw-inset-0 tw-overflow-hidden">
          <div class="tw-absolute tw-inset-0 tw-overflow-hidden">
            <div
              class="tw-pointer-events-none tw-fixed tw-inset-y-0 tw-right-0 tw-flex tw-max-w-full tw-pl-10"
            >
              <TransitionChild
                as="template"
                enter="tw-transform tw-transition tw-ease-in-out tw-duration-500 sm:tw-duration-700"
                enter-from="tw-translate-x-full"
                enter-to="tw-translate-x-0"
                leave="tw-transform tw-transition tw-ease-in-out tw-duration-500 sm:tw-duration-700"
                leave-from="tw-translate-x-0"
                leave-to="tw-translate-x-full"
              >
                <DialogPanel
                  class="tw-pointer-events-auto tw-relative tw-w-screen tw-max-w-md"
                >
                  <TransitionChild
                    as="template"
                    enter="tw-ease-in-out tw-duration-500"
                    enter-from="tw-opacity-0"
                    enter-to="tw-opacity-100"
                    leave="tw-ease-in-out tw-duration-500"
                    leave-from="tw-opacity-100"
                    leave-to="tw-opacity-0"
                  >
                    <div
                      class="tw-absolute tw-left-0 tw-top-0 -tw-ml-8 tw-flex tw-pr-2 tw-pt-4 sm:-tw-ml-10 sm:tw-pr-4"
                    >
                      <button
                        type="button"
                        class="tw-relative tw-rounded-md tw-text-gray-300 hover:tw-text-white focus:tw-outline-none focus:tw-ring-2 focus:tw-ring-white"
                        @click="showDrawer = false"
                      >
                        <span class="tw-absolute -tw-inset-2.5" />
                        <span class="tw-sr-only">Close panel</span>
                        <XMarkIcon class="tw-h-6 tw-w-6" aria-hidden="true" />
                      </button>
                    </div>
                  </TransitionChild>
                  <div
                    class="tw-flex tw-h-full tw-flex-col tw-overflow-y-scroll tw-bg-white tw-py-6 tw-shadow-xl"
                  >
                    <div class="tw-px-4 sm:tw-px-6">
                      <DialogTitle
                        class="tw-text-base tw-font-semibold tw-leading-6 tw-text-gray-900"
                      >
                        Send Feedback
                      </DialogTitle>
                    </div>
                    <div
                      class="tw-relative tw-mt-6 tw-flex-1 tw-px-4 sm:tw-px-6"
                    >
                      <!-- Feedback content -->
                      <CreateIssue />
                    </div>
                  </div>
                </DialogPanel>
              </TransitionChild>
            </div>
          </div>
        </div>
      </Dialog>
    </TransitionRoot>
  </footer>
</template>
