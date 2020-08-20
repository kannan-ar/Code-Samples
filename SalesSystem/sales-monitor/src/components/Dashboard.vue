<template>
  <div>Count - {{count}}</div>
</template>

<script lang="ts">
import { defineComponent, ref, onMounted } from "vue";
import axios from "axios";
import * as signalR from "@microsoft/signalr";

export default defineComponent({
  name: "Dashboard",
  setup() {
    const count = ref(0);

    onMounted(async () => {
      const connection = new signalR.HubConnectionBuilder()
        .withUrl("/salesHub")
        .build();

      connection.on("ReceiveSalesUpdate", function (purchase) {
        count.value++;
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