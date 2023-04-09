<script setup lang="ts">
import DoctorsCard from "../components/DoctorsCard.vue";
import { RouterLink } from "vue-router";
import { Doctor } from "../models/doctor";
import { ref } from "vue";
import { api_service } from "../services/api_service";

// const response = await fetch("https://localhost:7181/Doctors?page=0&size=10");

// console.log(response);

let doctors = ref<Doctor[]>([]);

// // const result = await response.json();

// // console.log(result);
const page = 0;
const size = 10;

doctors.value = await api_service.getData('doctors', { page, size});

// console.log(`list of doctors: ${doctors}`);

</script>

<template>
  <div class="flex justify-center">
    <h1
      class="text-center font-bold bg-gradient-to-r from-green-500 to-blue-500 text-transparent bg-clip-text text-6xl shadow-lg rounded-2xl p-4 hover:from-blue-500 hover:to-green-500 transition-all"
    >
      Doctors
    </h1>
  </div>
  <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-y-4" >
   
    <DoctorsCard
    v-for="item in doctors"
      :name="item.user!.fullName"
      :specialty="item.specialty"
    >
      <template v-slot:button>
        <RouterLink
          to="one"
          class="rounded-lg text-white bg-cyan-500 transition hover:bg-cyan-400 my-5 px-10 md:px-24 py-1"
        >
          Book Now
        </RouterLink>
      </template>
    </DoctorsCard>

    <DoctorsCard
      name="item.phone"
      specialty="item.specialty"
    >
      <template v-slot:button>
        <RouterLink
          to="one"
          class="rounded-lg text-white bg-cyan-500 transition hover:bg-cyan-400 my-5 px-10 md:px-24 py-1"
        >
          Book Now
        </RouterLink>
      </template>
    </DoctorsCard>
  </div>
</template>
