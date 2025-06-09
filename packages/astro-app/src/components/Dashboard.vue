<template>
  <main
    v-if="!shutdownApp"
    class="!tw-grid tw-h-full !tw-grid-cols-4 !tw-gap-3 tw-px-3 tw-py-1"
  >
    <div
      class="!tw-col-span-4 !tw-grid !tw-h-[20vh] !tw-grid-cols-4 !tw-gap-x-3"
    >
      <div>
        <Pages @showAlert="openAlert" :currentHost="currentHost" />
      </div>
      <div>
        <Medias @showAlert="openAlert" :currentHost="currentHost" />
      </div>
      <div>
        <Members @showAlert="openAlert" :currentHost="currentHost" />
      </div>
      <div>
        <Groups @showAlert="openAlert" :currentHost="currentHost" />
      </div>
    </div>
    <div class="!tw-col-span-4 !tw-grid tw-h-[23vh] !tw-grid-cols-3 tw-gap-x-3">
      <UserActivity @showAlert="openAlert" :currentHost="currentHost" />
      <DonutChart @showAlert="openAlert" :currentHost="currentHost" />
      <BarChart @showAlert="openAlert" :currentHost="currentHost" />
    </div>
    <div
      class="!tw-col-span-4 !tw-grid tw-h-[43vh] !tw-grid-cols-5 !tw-gap-x-3"
    >
      <LineChart @showAlert="openAlert" :currentHost="currentHost" />
      <Table @showAlert="openAlert" :currentHost="currentHost" />
    </div>
  </main>
  <Footer
    @showAlert="openAlert"
    :licensed="licensed"
    @licenseActivated="openPremiumFeatures"
    :computerName="computerName"
  />
  <FloatingActionButtons />
  <Alert
    :type="alertType"
    :message="alertMessage"
    :visible="isAlertVisible"
    @close="handleAlertClose"
    :duration="6000"
  />
</template>
<script setup lang="ts">
import { ref, onMounted } from "vue";
import Pages from "./cards/Pages.vue";
import Medias from "./cards/Medias.vue";
import Members from "./cards/Members.vue";
import Groups from "./cards/Groups.vue";
import UserActivity from "./cards/UserActivity.vue";
import DonutChart from "./cards/DonutChart.vue";
import BarChart from "./cards/BarChart.vue";
import LineChart from "./cards/LineChart.vue";
import Table from "./cards/Table.vue";
import FloatingActionButtons from "~/components/FloatingActionButtons.vue";
import Footer from "../components/Footer.vue";

const alertMessage = ref("Oops!");
const alertType = ref("alert");
const shutdownApp = ref(false);
const isAlertVisible = ref(false);
const lockPremiumFeatures = ref(false);
const licensed = ref(false);

const handleAlertClose = () => {
  isAlertVisible.value = false;
};

const shutdown = () => {
  shutdownApp.value = true;
  alertMessage.value = "Error found!";
  isAlertVisible.value = true;
};

function openAlert({ msg, type }) {
  alertMessage.value = msg;
  alertType.value = type;
  isAlertVisible.value = true;
}

const currentHost: string = import.meta.env.DEV
  ? "http://localhost:5000"
  : window.location.origin;

const computerName = ref("");

onMounted(async () => {});

function openPremiumFeatures() {
  lockPremiumFeatures.value = false;
  licensed.value = true;
  alertType.value = "success";
  alertMessage.value = "License was valid, well done!";
  isAlertVisible.value = true;
}
</script>
