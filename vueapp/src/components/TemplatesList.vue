<template>
    <div class="post">
        <h1>Templates</h1>

        <div v-if="!loading" class="content">
            <div v-for="(template, index) in this.post" :key="index" style="display:inline-block;margin-right:5px;">
                <div>
                    <router-link :to="'/template/' + index"><button style="width:200px; text-align:left;" class="button noRight">{{template.name}}</button></router-link>
                    <button @click="reimportProject(template.name)" class="button noRight noLeft">Reimport</button>
                    <button @click="deleteProject(template.guid, template.name)" class="button noLeft red">Delete</button>
                </div>
                <div class="trivia" style="margin-top:-18px; margin-bottom:12px;">
                    <p>0 fields</p>
                    <p>0 objects</p>
                    <p>Last updated: today</p>
                </div>
            </div>

            <ConfirmationModal v-if="confirmation" 
                               :Header="confirmation.header"
                               :Description="confirmation.description" 
                               @confirm="confirmation.confirmCallback()"
                               @cancel="confirmation.cancelCallback()" />
    </div>

    <h1>Enums</h1>
    <p>enum data here</p>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ConfirmationModal from './ConfirmationModal.vue'

    export default defineComponent({
        data() {
            return {
                loading: true,
                post: null,
                confirmation: null
            };
        },
        computed: {
        },
        components: {
            ConfirmationModal
        },
        created() {  
                this.fetchData();
        },
        watch: {
            '$route.params.id': function () { 
                this.fetchData();
            }
        },
        methods: {
            fetchData(viewProject) { 
                this.post = null;
                this.loading = true;

                fetch('/api/project' )
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        return;
                    });
            },
            deleteProject(project, projectName) {
                const thisRef = this;
                const guid = project;
                this.confirmation = {
                    header: "Delete " + projectName + "?",
                    description: "Are you sure you want to delete the project " + projectName + "(" + guid +")?",
                    confirmCallback: function () {
                        thisRef.confirmation = null;
                        fetch('/api/project/' + guid + '/delete', {
                            method: "DELETE",
                            headers: { "Content-Type": "application/json" }
                        })
                            .then(r => r.json())
                            .then(json => {
                                return;
                            });
                    },

                    cancelCallback: function () {
                        thisRef.confirmation = null;
                    }

                }
            },
            reimportProject(projectName) {
                const thisRef = this;
                this.confirmation = {
                    header: "Reimport " + projectName + "?",
                    description: "Are you sure you want to reimport the project " + projectName + "?",
                    confirmCallback: function () {
                        thisRef.confirmation = null;
                        fetch('/api/project/' + projectName + '/reimport', {
                            method: "PUT",
                            headers: { "Content-Type": "application/json" }
                        })
                            .then(r => r.json())
                            .then(json => {
                                return;
                            });
                    },

                    cancelCallback: function () {
                        thisRef.confirmation = null;
                    }

                }
            }
        },
    });
</script>

<style>
    .button {
        background-color: #27ae60;
        color: #fff;
        border: none;
        padding: 10px 20px;
        border-radius: 4px;
        font-size: 16px;
        cursor: pointer;
        transition: background-color 0.3s ease, transform 0.2s ease;
    }

        /* Click animation */
        .button:active {
            transform: scale(0.95);
        }

        .button:hover {
            background-color: #218c53;
        }

    .red {
        background-color: #e74c3c;
    }

        .red:hover{
            background-color: #c0392b;
        }

    .noLeft {
        border-bottom-left-radius: 0px;
        border-top-left-radius: 0px;
    }

    .noRight {
        border-bottom-right-radius: 0px;
        border-top-right-radius: 0px;
    }

    .trivia{
        background-color:#cccccc;
        width:376px;
        border-style:solid;
        border-top-style:none;
        color:black;
        border-color:black;
        margin-top:0px;
        padding-top:12px;
        padding-left:4px;
    }
</style>