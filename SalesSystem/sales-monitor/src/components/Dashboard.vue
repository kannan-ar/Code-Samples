<template>
<div><top-products /></div>
  <div>Count - {{ count }}</div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import axios from "axios";
import * as signalR from "@microsoft/signalr";

import ReportServices from "../ReportServices";
import TopProducts from './TopProducts.vue';

export default defineComponent({
  name: "Dashboard",
  components: {
    TopProducts
  },
  setup() {
    const count = ref(0);

    onMounted(async () => {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/salesHub")
        .build();

      connection.on("ReceiveSalesUpdate", function (purchase) {
        count.value++;
        if (count.value == 10) {
          ReportServices.map((x) => x.Log(purchase));
        }
      });

      await connection.start();
    });

    return {
      count,
    };
  },
});
</script>

<style scoped>
</style>