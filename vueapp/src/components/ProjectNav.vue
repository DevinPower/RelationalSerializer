<template>
    <div class="post">
        <h2 style="padding-left:20px;">Projects</h2>
        <div v-if="post" class="content scrollView" style="padding-left:20px">
            <div v-for="(project, index) in post" :key="index" style="margin-top: 4px; white-space: nowrap; width:100%;">
                <a @click="changeProject(index)" class="navEntry" style="cursor:pointer;">{{ project.name }}</a>
            </div>

        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        emits: ['update:project'],
        data() {
            return {
                loading: false,
                post: null
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
                        this.post = json;
                        this.loading = false;
                        return;
                    });
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