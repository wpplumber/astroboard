<template>
  <form @submit.prevent class="tw-mb-6">
    <div class="tw-mb-6">
      <label
        for="subject"
        class="tw-block tw-mb-2 tw-text-sm tw-font-medium tw-text-gray-900 dark:tw-text-white"
        >Subject</label
      >
      <input
        type="text"
        v-model="title"
        id="subject"
        class="tw-bg-gray-50 tw-border tw-border-gray-300 tw-text-gray-900 tw-text-sm tw-rounded-lg focus:tw-ring-blue-500 focus:tw-border-blue-500 tw-block tw-w-full tw-p-2.5 dark:tw-bg-gray-700 dark:tw-border-gray-600 dark:tw-placeholder-gray-400 dark:tw-text-white dark:focus:tw-ring-blue-500 dark:focus:tw-border-blue-500"
        placeholder="Let us know how we can help you"
        required
      />
    </div>
    <div class="tw-mb-6">
      <label
        for="message"
        class="tw-block tw-mb-2 tw-text-sm tw-font-medium tw-text-gray-900 dark:tw-text-white"
        >Your message</label
      >
      <textarea
        id="message"
        v-model="body"
        rows="4"
        class="tw-block tw-p-2.5 tw-w-full tw-text-sm tw-text-gray-900 tw-bg-gray-50 tw-rounded-lg tw-border tw-border-gray-300 focus:tw-ring-blue-500 focus:tw-border-blue-500 dark:tw-bg-gray-700 dark:tw-border-gray-600 dark:tw-placeholder-gray-400 dark:tw-text-white dark:focus:tw-ring-blue-500 dark:focus:tw-border-blue-500"
        placeholder="Your message..."
      ></textarea>
    </div>
    <button
      type="button"
      @click="createIssue"
      class="tw-text-white tw-bg-blue-700 hover:tw-bg-blue-800 tw-w-full focus:tw-ring-4 focus:tw-ring-blue-300 tw-font-medium tw-rounded-lg tw-text-sm tw-px-5 tw-py-2.5 tw-mb-2 dark:tw-bg-blue-600 dark:hover:tw-bg-blue-700 focus:tw-outline-none dark:focus:tw-ring-blue-800 tw-block"
    >
      Send message
    </button>
  </form>
</template>

<script setup lang="ts">
import { onMounted, ref, watch } from "vue";

// const props = defineProps<{
//     isOpen: boolean;
// }>();

// const emit = defineEmits(["update:isOpen"]);

const title = ref("");
const body = ref("");
const errorMessage = ref<string>("");

const createIssue = async () => {
  try {
    const response = await fetch(
      "https://api.github.com/repos/wpplumber/astroboard/issues",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/vnd.github.v3+json",
          Authorization: `Bearer ${import.meta.env.VITE_GITHUB_TOKEN}`,
        },
        body: JSON.stringify({
          title: title.value,
          body: body.value,
          assignees: ["wpplumber"],
          milestone: 1,
          labels: ["bug"],
        }),
      },
    );

    if (!response.ok) {
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    alertMessage.value = "Issue created successfully";
    alertStyle.value = "success";
    isAlertVisible.value = true;
  } catch (error) {
    alertMessage.value = "Request was invalid!";
    alertStyle.value = "error";
    isAlertVisible.value = true;
  }
};
</script>
