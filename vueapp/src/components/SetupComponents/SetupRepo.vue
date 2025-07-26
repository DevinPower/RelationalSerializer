<template>
  <div v-if="repositories">
    <div class="mt-3 text-center sm:mt-5">
      <div class="mt-2">
          <Listbox as="div" v-model="selected">
            <ListboxLabel class="block text-sm/6 font-medium text-gray-900">Repository</ListboxLabel>
            <div class="relative mt-2">
              <ListboxButton class="grid w-full cursor-default grid-cols-1 rounded-md bg-white py-1.5 pr-2 pl-3 text-left text-gray-900 outline-1 -outline-offset-1 outline-gray-300 focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6">
                <span class="col-start-1 row-start-1 truncate pr-6">{{ selected.name }}</span>
                <ChevronUpDownIcon class="col-start-1 row-start-1 size-5 self-center justify-self-end text-gray-500 sm:size-4" aria-hidden="true" />
              </ListboxButton>

              <transition leave-active-class="transition ease-in duration-100" leave-from-class="opacity-100" leave-to-class="opacity-0">
                <ListboxOptions class="absolute z-10 mt-1 max-h-60 w-full overflow-auto rounded-md bg-white py-1 text-base shadow-lg ring-1 ring-black/5 focus:outline-hidden sm:text-sm">
                  <ListboxOption as="template" v-for="repository in repositories" :key="repository" :value="repository" v-slot="{ active, selected }">
                    <li :class="[active ? 'bg-indigo-600 text-white outline-hidden' : 'text-gray-900', 'relative cursor-default py-2 pr-9 pl-3 select-none']">
                      <span :class="[selected ? 'font-semibold' : 'font-normal', 'block truncate']">{{ repository.name }}</span>

                      <span v-if="selected" :class="[active ? 'text-white' : 'text-indigo-600', 'absolute inset-y-0 right-0 flex items-center pr-4']">
                        <CheckIcon class="size-5" aria-hidden="true" />
                      </span>
                    </li>
                  </ListboxOption>
                </ListboxOptions>
              </transition>
            </div>
          </Listbox>
      </div>
    </div>
  </div>
  <div class="mt-5 sm:mt-6 flex justify-end">
    <button type="button" class="mt-3 inline-flex justify-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50 sm:mt-0" @click="validateToken">Next</button>
  </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import { Listbox, ListboxButton, ListboxLabel, ListboxOption, ListboxOptions } from '@headlessui/vue'
    import { ChevronUpDownIcon } from '@heroicons/vue/16/solid'
    import { CheckIcon } from '@heroicons/vue/20/solid'

    export default defineComponent({
        props: ['continueCallback', 'errorCallback', 'backCallback'],
        emits :[],
        data() {
            return {
              repositories: null,
              selected: null
            };
        },
        components: {
          Listbox, ListboxButton, ListboxLabel, ListboxOption, ListboxOptions,
          ChevronUpDownIcon, CheckIcon
        },
        created() {
          fetch('/api/onboard/listrepositories' )
            .then(r => r.json())
            .then(json => {
                this.repositories = json;
                this.selected = this.repositories[0];
                return;
            });
        },
        watch: {
        },
        methods: {
        },
    });
</script>