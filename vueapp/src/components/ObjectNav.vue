<template>
    <div class="post fixed top-0" style="overflow-x:hidden; width:200px;">
        <div class="flex items-center my-4">
            <div class="relative flex items-center justify-between">
                <span class="bg-gray pr-3 text-base font-semibold text-gray-900">{{ projectName }}</span>
            </div>
            <div class="flex-grow border-t border-gray-300" />
            <button @click="makeObject()" type="button" class="inline-flex items-center gap-x-1.5 rounded-full bg-white px-3 py-1.5 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50">
                <PlusIcon class="-mr-0.5 -ml-1 size-5 text-gray-400" aria-hidden="true" />
                <span>Compose</span>
            </button>
        </div>

        <div class="flex flex-1 w-full justify-center px-2">
            <div class="grid w-full grid-cols-1">
                <input type="search" name="search" aria-label="Search" class="col-start-1 row-start-1 block w-full rounded-md bg-gray-700 py-1.5 pr-3 pl-10 text-base text-white outline-hidden placeholder:text-gray-400 focus:bg-white focus:text-gray-900 focus:placeholder:text-gray-400 sm:text-sm/6" 
                    placeholder="Search" v-model="searchText" />
                <MagnifyingGlassIcon class="pointer-events-none col-start-1 row-start-1 ml-3 size-5 self-center text-gray-400" aria-hidden="true" />
            </div>
        </div>

        <div style="height:16px;" />

        <div style="overflow-y:scroll; direction: rtl; max-height: calc(100vh - 116px);">
            <div v-for="object in post" :key="object.guid" class="w-full" style="direction:ltr;">
                <router-link
                    v-if="object.name.toUpperCase().includes(searchText.toUpperCase())"
                    class="w-full block"
                    :to="{ path: '/edit/' + project + '/' + object.guid }"
                    @contextmenu.prevent.stop="handleClick($event, object.guid)"
                    :style="{
                        color: object.exportExcluded ? '#636363' : 'all',
                        backgroundColor: (id == object.guid || hoverGuid === object.guid) ? 
                                            '#f3f4f6' : 'inherit',
                        padding: '0',
                        marginLeft: '0px',
                        marginRight: '0px',
                        width: '100%',
                        'overflow-x': 'hidden'
                    }"
                    @mouseover="hoverGuid = object.guid"
                    @mouseout="hoverGuid = null"
                >
                    <span>
                        <small>{ </small>
                        <span>{{ object.name }}</span>
                        <small> }</small>
                    </span>
                </router-link>
            </div>
        </div>

    </div>

    <vue-simple-context-menu element-id="contextMenu"
                             :options="contextArray"
                             ref="vueSimpleContextMenu"
                             @option-clicked="optionClicked" />
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import 'vue-simple-context-menu/dist/vue-simple-context-menu.css';
    import {  PlusIcon, MagnifyingGlassIcon
        } from '@heroicons/vue/24/outline';

    export default defineComponent({
        props: ['project', 'id', 'editingText'],
        data() {
            return {
                loading: false,
                post: [],
                projectName: "",
                searchText: "",
                contextGuid: "",
                hoverGuid: null,
                contextArray: [
                    {
                        name: 'Duplicate', value: (item) => {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/duplicate', {
                                method: "PUT",
                                headers: { "Content-Type": "application/json" }
                            }).then(r => {
                                r.json().then(
                                    newObject => {
                                        this.post.push(newObject)
                                });
                            });
                        }
                    },
                    {
                        name: 'Exclude', value: (item) => {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/exporttoggle', {
                                method: "PATCH",
                                headers: { "Content-Type": "application/json" }
                            });
                        }
                    },
                    {
                        name: 'Delete', value: (item) => {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/delete', {
                                method: "DELETE",
                                headers: { "Content-Type": "application/json" }
                            }).then(x =>{
                                if (x.status == 200){
                                    this.post = this.post.filter(obj => obj.guid !== item.guid);
                                    if (item.guid == this.id)
                                        this.$router.push({ path: `/edit/${this.project}/undefined` });
                                }
                            });
                        }
                    },
                    {
                        name: 'Copy Guid', value: (item) => {
                            navigator.clipboard.writeText(item.guid);
                        }
                    }
                ],
            };
        },
        created() {
            // fetch the data when the view is created and the data is
            // already being observed
            this.fetchData();
        },
        components: {
            PlusIcon, MagnifyingGlassIcon
        },
        watch: {
            project(newValue, oldValue) { 
               this.fetchData();
            }
        },
        methods: {
            fetchData() {
                this.post = null;
                this.loading = true;

                fetch('/api/project/' + this.project + '/objects')
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        return;
                    });

                fetch('/api/project/' + this.project + '/name')
                    .then(r => r.text())
                    .then(text => {
                        this.projectName = text;
                        return;
                    });
            },
            makeObject() {
                fetch('/api/project/' + this.project + '/objects', {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" }
                })
                .then(r => {
                    r.json().then(
                        newObject => {
                            this.post.push(newObject)
                    });
                });
            },
            handleClick(event, item) {
                const itemWrapper = { project: this.project, guid: item };
                this.$refs.vueSimpleContextMenu.showMenu(event, itemWrapper);
            },
            optionClicked(event) {
                event.option.value(event.item);
            }
        },
    });
</script>