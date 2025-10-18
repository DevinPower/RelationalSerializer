<template>
  <div class="px-4 sm:px-6 lg:px-8">
    <div class="mt-8 flow-root">
      <div class="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
        <div class="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
          <div class="overflow-hidden shadow-sm ring-1 ring-black/5 sm:rounded-lg">
            <table class="min-w-full divide-y divide-gray-300">
              <thead class="bg-gray-50">
                <tr>
                  <th scope="col" class="py-3.5 pr-3 pl-4 text-left text-sm font-semibold text-gray-900 sm:pl-6">Filename</th>
                  <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">Count</th>
                  <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900 text-right">Manage</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200 bg-white">
                <tr v-for="(importedClass, index) in importedClasses" :key="importedClass.name">
                  <td class="py-4 pr-3 pl-4 text-sm font-medium whitespace-nowrap text-gray-900 sm:pl-6">{{ importedClass.name }}</td>
                  <td class="px-3 py-4 text-sm whitespace-nowrap text-gray-500">--</td>
                  <td class="relative py-4 pr-4 pl-3 text-right text-sm font-medium whitespace-nowrap sm:pr-6">
                    <span class="isolate inline-flex rounded-md shadow-xs">
                      <button @click="changeRouteToTemplate(index)" type="button" class="relative inline-flex items-center rounded-l-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 ring-1 ring-gray-300 ring-inset hover:bg-gray-50 focus:z-10">Edit</button>
                      <button @click="reimportTemplate(index, importedClass.name)" type="button" class="relative -ml-px inline-flex items-center bg-white px-3 py-2 text-sm font-semibold text-gray-900 ring-1 ring-gray-300 ring-inset hover:bg-gray-50 focus:z-10">Reimport</button>
                      <button @click="deleteTemplate(index, importedClass.name)" type="button" class="relative -ml-px inline-flex items-center rounded-r-md bg-red-200 px-3 py-2 text-sm font-semibold text-red-900 ring-1 ring-red-300 ring-inset hover:bg-red-100 focus:z-10">Delete</button>
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
  <ConfirmationModal v-if="confirmation" 
                   :Header="confirmation.header"
                   :Description="confirmation.description" 
                   @confirm="confirmation.confirmCallback()"
                   @cancel="confirmation.cancelCallback()" />
</template>

<script lang="js">
    import { defineComponent } from 'vue';
  import ConfirmationModal from './ConfirmationModal.vue'

    export default defineComponent({
        props: ['importedClasses'],
        data() {
            return {
              confirmation: null
            };
        },
        components: {
          ConfirmationModal
        },
        created() {  
        },
        watch: {
        },
        methods: {
          changeRouteToTemplate(index){
            this.$router.push(`/template/${index}`);
          },
          reimportTemplate(index, projectName){
            const thisRef = this;
            this.confirmation = {
                header: "Reimport " + projectName + "?",
                description: "Are you sure you want to reimport the project " + projectName + "?",
                confirmCallback: function () {
                    thisRef.confirmation = null;
                    fetch('/api/project/' + projectName + '/reimport', {
                        method: "PUT",
                        headers: { "Content-Type": "application/json" }
                    });
                },
                cancelCallback: function () {
                    thisRef.confirmation = null;
                }
            }
          },
          deleteTemplate(index, projectName){

          }
        },
    });
</script>