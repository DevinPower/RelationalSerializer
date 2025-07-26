<template>
    <TransitionRoot as="template" :show="open">
      <Dialog class="relative z-10" @close="open = false">
        <TransitionChild as="template" enter="ease-out duration-300" enter-from="opacity-0" enter-to="opacity-100" leave="ease-in duration-200" leave-from="opacity-100" leave-to="opacity-0">
          <div class="fixed inset-0 bg-gray-500/75 transition-opacity" />
        </TransitionChild>

        <div class="fixed inset-0 z-10 w-screen overflow-y-auto">
          <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
            <TransitionChild as="template" enter="ease-out duration-300" enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95" enter-to="opacity-100 translate-y-0 sm:scale-100" leave="ease-in duration-200" leave-from="opacity-100 translate-y-0 sm:scale-100" leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95">
              <DialogPanel class="relative transform overflow-hidden rounded-lg bg-white px-4 pt-5 pb-4 text-left shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-lg sm:p-6">
                
                <BreadcrumbNav style="margin-left:-5%;margin-right:-5%;margin-top:-5%;margin-bottom: 2%;"
                    :pages="setupPages"/>    
                
                <AlertBox v-if="alertText" :text="alertText"/>
                <component :is="pageComponents[currentPage]" 
                    :continueCallback="nextPage"
                    :errorCallback="setError"
                    :backCallback="setupPages[currentPage].backCallback"
                    />


              </DialogPanel>
            </TransitionChild>
          </div>
        </div>
      </Dialog>
    </TransitionRoot>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue';
    import { CheckIcon } from '@heroicons/vue/24/outline';
    import BreadcrumbNav from './BreadcrumbNav.vue';
    import AlertBox from './AlertBox.vue'
    import SetupInfo from './SetupComponents/SetupInfo.vue';
    import SetupAPI from './SetupComponents/SetupAPI.vue';
import SetupRepo from './SetupComponents/SetupRepo.vue';

    export default defineComponent({
        props: ['open'],
        emits :[],
        data() {
            return {
                currentPage: 0,
                setupPages:[
                    {name: 'Info', current: true, backCallback: () => {} },
                    {name: 'API Key', current: false, backCallback: () => {
                      this.alertText = "";
                      this.currentPage--;
                    }},
                    {name: 'Repository', current: false, backCallback: () => {} }
                ],
                pageComponents: ['SetupInfo', 'SetupAPI', 'SetupRepo'],
                alertText: null
            };
        },
        components: {
            Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot,
            CheckIcon,
            BreadcrumbNav, AlertBox, SetupInfo, SetupAPI, SetupRepo
        },
        created() {
        },
        watch: {
        },
        methods: {
            nextPage(){
                this.alertText = "";
                this.setupPages[this.currentPage].current = false;
                this.currentPage++;
                this.setupPages[this.currentPage].current = true;
            },
            setError(text){
              this.alertText = text;
            }
        },
    });
</script>