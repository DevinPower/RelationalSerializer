<template>
    <ImportedClassesTable v-if="importedClasses"
        :importedClasses="importedClasses"/>

    <div style="height:16px"></div>

    <div class="post" v-if="!post">
        <RepoBrowser Header="Import" 
            Prompt="Select source files from below to include them as a project."
            :directory="directory"
            @cd="selectFolder"
            @updir="upDir"
            @select="selectFile"></RepoBrowser>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ImportedClassesTable from './ImportedClassesTable.vue';
    import RepoBrowser from './RepoBrowser.vue';

    export default defineComponent({
        data() {
            return {
                directory: [],
                cd: '/',
                importedClasses: null
            };
        },
        components: {
            RepoBrowser, ImportedClassesTable
        },
        created() {
            this.getPath();
            this.getExistingClasses();
        },
        watch: {
        },
        methods: {
            selectFolder(appendValue){
                this.cd += '/' + appendValue;
                this.getPath();
            },
            upDir(){
                if (this.cd === '/' || this.cd === '') return;
                // Remove trailing slash if present (except root)
                let path = this.cd.replace(/\/$/, '');
                // Remove last segment
                path = path.substring(0, path.lastIndexOf('/'));
                // Ensure at least '/'
                this.cd = path === '' ? '/' : path;
                this.getPath();
            },
            getPath(){
                fetch(`/api/project/importable?Path=${this.cd}` )
                    .then(r => r.json())
                    .then(json => {
                        this.directory = json;
                        this.loading = false;
                        return;
                    });
            },
            selectFile(filepath){
                fetch('/api/project/import', {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.cd + '/' + filepath)
                    });
            },
            getExistingClasses(){
            fetch('/api/project' )
                .then(r => r.json())
                .then(json => {
                    this.importedClasses = json;
                    return;
                });
            }
        },
    });
</script>

<style>
    
</style>