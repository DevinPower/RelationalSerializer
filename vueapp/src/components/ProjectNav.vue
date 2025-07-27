<template>
  <div class="flex grow flex-col gap-y-5 overflow-y-auto bg-indigo-600 px-6">
  <nav class="flex flex-1 flex-col" aria-label="Sidebar"
  style="padding:16px;">
    <ul role="list" class="flex flex-1 flex-col gap-y-7">
      <li>
        <ul role="list" class="-mx-2 space-y-1">
          <li v-for="item in navigation" :key="item.name">
            <a :href="item.href" :class="[item.current ? 'bg-indigo-700 text-white' : 'text-indigo-200 hover:bg-indigo-700 hover:text-white', 'group flex gap-x-3 rounded-md p-2 text-sm/6 font-semibold']">
              <component :is="item.icon" :class="[item.current ? 'text-white' : 'text-indigo-200 group-hover:text-white', 'size-6 shrink-0']" aria-hidden="true" />              {{ item.name }}
            </a>
          </li>
        </ul>
      </li>
      <li>
        <div class="text-xs/6 font-semibold text-gray-400">Projects</div>
        <ul role="list" class="-mx-2 mt-2 space-y-1">
          <li v-for="(item, index) in secondaryNavigation" :key="item.name">
              <a @click="changeProject(index)" :href="item.href" :class="[item.current ? 'bg-indigo-700 text-white' : 'text-indigo-200 hover:bg-indigo-700 hover:text-white', 'group flex gap-x-3 rounded-md p-2 text-sm/6 font-semibold']">
                <span class="flex size-6 shrink-0 items-center justify-center rounded-lg border border-indigo-400 bg-indigo-500 text-[0.625rem] font-medium text-white">{{ item.name[0] }}</span>
                <span class="truncate">{{ item.name }}</span>
            </a>
          </li>
        </ul>
      </li>
    </ul>
  </nav>
</div>
</template>

<script>
import { defineComponent } from 'vue';
import {
  HomeIcon,
  CodeBracketSquareIcon,
  DocumentTextIcon,
  PlusIcon
} from '@heroicons/vue/24/outline'

    export default defineComponent({
        emits: ['update:project'],
        data() {
            return {
                loading: false,
                navigation : [
                  { name: 'Dashboard', href: '#', icon: HomeIcon, current: true },
                  { name: 'Import', href: '#', icon: CodeBracketSquareIcon, current: false },
                  { name: 'Templates', href: '#', icon: DocumentTextIcon, current: false }
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