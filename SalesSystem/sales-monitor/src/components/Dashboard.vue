<template>
    <div>Test</div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import axios from "axios";
import * as signalR from "@microsoft/signalr";

export default defineComponent({
  name: 'Dashboard',
  created: async () => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("/salesHub")
      .build();
    connection.on("ReceiveSalesUpdate", function (purchase) {
      console.log(purchase);
    });

    await connection.start();
    const res = await axios.get("/Demo");
    console.log(res);
  }
});
</script>

<style scoped>

</style>