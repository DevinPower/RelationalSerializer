<template>
    <div class="post" v-if="!post">
        <input type="file" id="newClass" name="filename" @change="AddFile" style="width:80%;">
        <FC_Code v-model="fileContent" style="width:80%;" additionalData="{language: 'xml'}"></FC_Code>
        <button @click="SendFile(this.fileContent)">Let's go!</button>
    </div>
    <div v-else>
        {{this.post}}
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import FC_Code from './FieldComponents/FC_Code.vue'

    export default defineComponent({
        data() {
            return {
                loading: false,
                fileContent : ""
            };
        },
        components: {
            FC_Code
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