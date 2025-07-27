<template>
    <div class="post" v-if="!post">
        <reference-modal Header="Import" 
        Prompt="Select source files from below to include them as a project."
        :-options="directory"
        -hide-cancel="true"></reference-modal>
    </div>
    <div v-else>
        {{this.post}}
    </div>

    <ImportedClassesTable></ImportedClassesTable>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ReferenceModal from './ReferenceModal.vue'
    import ImportedClassesTable from './ImportedClassesTable.vue';

    export default defineComponent({
        data() {
            return {
                directory: [],
                cd: '/'
            };
        },
        components: {
            ReferenceModal, ImportedClassesTable
        },
        created() {
            this.GetPath();
        },
        watch: {
        },
        methods: {
            selectFolder(appendValue){
                this.cd += '/' + appendValue;
                this.GetPath();
            },
            GetPath(){
                fetch(`/api/project/importable?Path=${this.cd}` )
                    .then(r => r.json())
                    .then(json => {
                        this.directory = json;
                        this.loading = false;
                        return;
                    });
            },
            //TODO: Kill both these functions, import file from backend directly from GH
            AddFile(event){
                const reader = new FileReader();
                reader.onload = (res) => {
                  this.fileContent = res.target.result;
                };

                reader.readAsText(event.target.files[0]);
            },
            SendFile(fileContents) { 
                fetch('/api/project/create',{
                    method: "PUT",
                    body: JSON.stringify(fileContents),
                    headers: { "Content-Type": "application/json" }
                }).then((response) => response.text())
                    .then((x) => this.post = x);
            }
        },
    });
</script>

<style>
    
</style>