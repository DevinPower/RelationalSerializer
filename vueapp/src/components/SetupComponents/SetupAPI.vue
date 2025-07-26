<template>
  <div>
    <div class="mt-3 text-center sm:mt-5">
      <div class="mt-2">
        
        <span class="bg-white pr-3 text-base font-semibold text-gray-900">API Key</span>
        <div class="border-b border-gray-200 pb-px focus-within:border-b-2 focus-within:border-indigo-600 focus-within:pb-0">
          <input v-model="apiKey" rows="3" name="apiKey" id="apiKey" class="block w-full resize-none text-base text-gray-900 placeholder:text-gray-400 focus:outline-none sm:text-sm/6" />
        </div>

      </div>
    </div>
  </div>
  <div class="mt-5 sm:mt-6 flex justify-end">
    <button type="button" class="mt-3 inline-flex justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50 sm:mt-0" @click="validateToken">Next</button>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['continueCallback', 'errorCallback', 'backCallback'],
        emits :[],
        data() {
            return {
              apiKey: ''
            };
        },
        components: {
        },
        created() {
        },
        watch: {
        },
        methods: {
          validateToken(){
            fetch('/api/onboard/repo', {
              method: 'POST',
              headers: { 'Content-Type': 'application/json' },
              body: JSON.stringify(this.apiKey)
            } )
              .then(r => {
                if (r.status != 200){
                  r.text().then(error =>{
                    this.errorCallback(error);
                  })
                }

                this.continueCallback();
              });
          }
        },
    });
</script>