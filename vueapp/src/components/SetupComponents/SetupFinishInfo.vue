<template>
  <div>
    <div class="mt-3 text-center sm:mt-5">
      <DialogTitle as="h3" class="text-base font-semibold text-gray-900">All set! 🎉</DialogTitle>
      <div class="mt-2">
        <p class="text-sm text-gray-500">
        Your workspace is setup and ready to go. Why not get started with some of the things below?  
        </p>
        <ul>
          <li>Import classes from your repository</li>
          <li>Configure more settings</li>
          <li>Some other cool thing?</li>
        </ul>
      </div>
    </div>
  </div>
  <div class="mt-5 sm:mt-6 sm:grid sm:grid-flow-row-dense sm:grid-cols-2 sm:gap-3">
    <button type="button" class="mt-3 inline-flex w-full justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50 sm:col-start-1 sm:mt-0" @click="continueCallback">Finish</button>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['continueCallback', 'errorCallback', 'backCallback'],
        emits :[],
        data() {
            return {
            };
        },
        components: {
        },
        created() {
        },
        watch: {
        },
        methods: {
        },
    });
</script>