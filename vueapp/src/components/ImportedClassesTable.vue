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
                <tr v-for="importedClass in importedClasses" :key="importedClass.name">
                  <td class="py-4 pr-3 pl-4 text-sm font-medium whitespace-nowrap text-gray-900 sm:pl-6">{{ importedClass.name }}</td>
                  <td class="px-3 py-4 text-sm whitespace-nowrap text-gray-500">{{ importedClass.count }}</td>
                  <td class="relative py-4 pr-4 pl-3 text-right text-sm font-medium whitespace-nowrap sm:pr-6">
                    <a href="#" class="text-indigo-600 hover:text-indigo-900">Edit</a>
                    |
                    <a href="#" class="text-indigo-600 hover:text-indigo-900">Remove</a>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: [],
        data() {
            return {
              importedClasses: [  { name: 'test.cs', count: 213 }]
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