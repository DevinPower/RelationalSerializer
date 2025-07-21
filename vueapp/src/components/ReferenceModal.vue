<template>
    <div class="modal-mask" style="height:100%">
        <div class="modal-wrapper" style="min-height: 300px;">
            <div class="modal-container" style="height:100%;">
                <div class="border-b border-gray-200 pb-5">
                    <h3 class="text-base font-semibold text-gray-900">{{ this.Header }}</h3>
                    <p class="mt-2 max-w-4xl text-sm text-gray-500">{{ this.Prompt }}</p>
                </div>
                <div>

                <div class="flex flex-1 justify-center px-2 lg:ml-6 lg:justify-end">
                    <div class="grid w-full grid-cols-1">
                        <input type="search" name="search" aria-label="Search" class="col-start-1 row-start-1 block w-full rounded-md bg-gray-700 py-1.5 pr-3 pl-10 text-base text-white outline-hidden placeholder:text-gray-400 focus:bg-white focus:text-gray-900 focus:placeholder:text-gray-400 sm:text-sm/6" placeholder="Search" />
                        <MagnifyingGlassIcon class="pointer-events-none col-start-1 row-start-1 ml-3 size-5 self-center text-gray-400" aria-hidden="true" />
                    </div>
                </div>

                <nav class="flex flex-1 flex-col" aria-label="Sidebar">
                    <ul role="list" class="-mx-2 space-y-1">
                    <li v-for="item in Options" :key="item.guid">
                        <a :href="item.href" :class="['text-gray-700 hover:bg-gray-50 hover:text-indigo-600', 'group flex gap-x-3 rounded-md p-2 pl-3 text-sm/6 font-semibold']">{{ item.name }}</a>
                    </li>
                    </ul>
                </nav>

                </div>
                 <div v-if="!HideCancel" class="flex justify-end">
                   <button @click="$emit('cancel')" type="button" class="rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50">Cancel</button>
                 </div>
            </div>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import {  MagnifyingGlassIcon
        } from '@heroicons/vue/24/outline';

    export default defineComponent({
        props: ['Header', 'Prompt', 'Options', 'HideCancel'],
        emits :[],
        data() {
            return {
            };
        },
        components: {
            MagnifyingGlassIcon
        },
        created() {
        },
        watch: {
        },
        methods: {
        },
    });
</script>

<style>
    .modal-container{
        color:black;
    }
</style>