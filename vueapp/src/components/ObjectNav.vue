<template>
    <div class="post">
        <div v-if="post" class="content scrollView" style="padding-left:20px">
            <h1>{{this.projectName}}</h1>
            <center>
                <input type="search" v-model="searchText" placeholder="search..." />
                <button class="compose-button" @click="makeObject()" style="width: 100%; white-space: nowrap; margin-top:12px;">
                    <i class="fas fa-pencil-alt"></i> Compose
                </button>
            </center>

            <div v-for="object in post" :key="object" style="margin-top: 4px; white-space: nowrap; width:100%;">
                <router-link v-if="object.name.toUpperCase().includes(searchText.toUpperCase())"
                             class="navEntry" :to="{ path: '/edit/' + project + '/' + object.guid }"
                             @contextmenu.prevent.stop="handleClick($event, object.guid)"
                             :style="{ color: object.exportExcluded ? '#636363' : 'all' }"><small>{</small> {{ object.name }} <small>}</small></router-link>
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

    export default defineComponent({
        props: ['project'],
        data() {
            return {
                loading: false,
                post: null,
                projectName: "",
                searchText: "",
                contextGuid: "",
                contextArray: [
                    {
                        name: 'Duplicate', value: function (item) {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/duplicate', {
                                method: "PUT",
                                headers: { "Content-Type": "application/json" }
                            });
                        }
                    },
                    {
                        name: 'Exclude', value: function (item) {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/exporttoggle', {
                                method: "PATCH",
                                headers: { "Content-Type": "application/json" }
                            });
                        }
                    },
                    {
                        name: 'Delete', value: function (item) {
                            fetch('/api/object/' + item.project + '/' + item.guid + '/delete', {
                                method: "DELETE",
                                headers: { "Content-Type": "application/json" }
                            });
                        }
                    },
                    {
                        name: 'Copy Guid', value: function (item) {
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
                .then(this.fetchData());
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

<style>
    .scrollView {
        height: 100vh;
        overflow-y:auto;
    }

    body {
        margin: 0;
        padding: 0;
        font-family: Arial, sans-serif;
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100vh;
        background-color: #f0f0f0;
    }

    .navEntry {
        display: inline-block;
        width:100%;
    }

        .navEntry:hover {
            background-color: #818181;
            color: #333333;
        }

    .compose-button {
        background-color: #27ae60;
        color: #fff;
        border: none;
        padding: 10px 20px;
        border-radius: 4px;
        font-size: 16px;
        cursor: pointer;
        transition: background-color 0.3s ease, transform 0.2s ease;
    }

        .compose-button i {
            margin-right: 8px;
        }

        /* Click animation */
        .compose-button:active {
            transform: scale(0.95);
        }

        .compose-button:hover {
            background-color: #218c53;
        }
</style>