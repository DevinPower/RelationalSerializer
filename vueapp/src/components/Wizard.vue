<template>
    <div class="post" v-if="!post">
        <reference-modal Header="Import" 
        Prompt="Select source files from below to include them as a project."
        :-options="[{'guid' : 'test', 'name': 'test1'},
            {'guid' : 'meg', 'name': 'src/data/cards.cs'}
        ]"
        -hide-cancel="true"></reference-modal>
    </div>
    <div v-else>
        {{this.post}}
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ReferenceModal from './ReferenceModal.vue'

    export default defineComponent({
        data() {
            return {
                loading: false,
                fileContent : ""
            };
        },
        components: {
            ReferenceModal
        },
        created() {
        },
        watch: {
        },
        methods: {
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