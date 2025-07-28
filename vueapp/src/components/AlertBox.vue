<template>
  <div class="rounded-md bg-red-50 p-4">
    <div class="flex">
      <div class="shrink-0">
        <XCircleIcon class="size-5 text-red-400" aria-hidden="true" />
      </div>
      <div class="ml-3">
        <h3 class="text-sm font-medium text-red-800">{{ text }}</h3>
      </div>
    </div>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import { XCircleIcon } from '@heroicons/vue/20/solid';

    export default defineComponent({
        props: ['text'],
        emits :[],
        data() {
            return {
            };
        },
        components: {
          XCircleIcon
        },
        created() {
        },
        watch: {
        },
        methods: {
        },
    });
</script>