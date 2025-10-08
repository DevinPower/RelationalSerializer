<template>
  <div class="min-h-screen flex flex-col fixed top-0">
    <div class="flex grow flex-col gap-y-5 overflow-y-auto bg-stone-800 px-6">
      <nav class="flex flex-1 flex-col" aria-label="Sidebar" style="padding:16px;">
        <ul role="list" class="flex flex-1 flex-col gap-y-7">
          <li>
            <ul role="list" class="-mx-2 space-y-1">
              <li v-for="item in navigation" :key="item.name">
                <router-link :to="item.href" :class="[item.current ? 'bg-stone-700 text-white' : 'text-stone-200 hover:bg-stone-700 hover:text-white', 'group flex gap-x-3 rounded-md p-2 text-sm/6 font-semibold']">
                  <component :is="item.icon" :class="[item.current ? 'text-white' : 'text-stone-200 group-hover:text-white', 'size-6 shrink-0']" aria-hidden="true" />{{ item.name }}
                </router-link>
              </li>
            </ul>
          </li>
          <li>
            <div class="text-xs/6 font-semibold text-gray-400">Projects</div>
            <ul role="list" class="-mx-2 mt-2 space-y-1">
              <li v-for="(item, index) in secondaryNavigation" :key="item.name">
                  <a style="cursor: pointer;" @click="changeProject(index)" :href="item.href" :class="[selectedIndex == index ? 'bg-stone-700 text-white' : 'text-stone-200 hover:bg-stone-700 hover:text-white', 'group flex gap-x-3 rounded-md p-2 text-sm/6 font-semibold']">
                    <span class="flex size-6 shrink-0 items-center justify-center rounded-lg border border-stone-400 bg-stone-500 text-[0.625rem] font-medium text-white">{{ item.name[0] }}</span>
                    <span class="truncate">{{ item.name }}</span>
                </a>
              </li>
            </ul>
          </li>
        </ul>
      </nav>
    </div>
  </div>
</template>

<script>
import { defineComponent } from 'vue';
import {
  HomeIcon,
  CodeBracketSquareIcon
} from '@heroicons/vue/24/outline'

    export default defineComponent({
        emits: ['update:project'],
        props: ['selectedIndex'],
        data() {
            return {
                loading: false,
                navigation : [
                  { name: 'Import', href: '/import', icon: CodeBracketSquareIcon, current: false }
                ],
                secondaryNavigation: [
                ]
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        watch: {
            // call again the method if the route changes
            //'$route': 'fetchData'
        },
        methods: {
            changeProject(newID) {
                this.$emit('update:project', newID);
            },
            fetchData() {
                this.post = null;
                this.loading = true;

                fetch('/api/project' )
                    .then(r => r.json())
                    .then(json => {
                        this.secondaryNavigation = json;
                        this.secondaryNavigation[0].current = true;
                        this.loading = false;
                        return;
                    });
            }
        },
    });
</script>