<template>
    <div class="modal-mask" style="height:100%">
        <div class="modal-wrapper" style="min-height: 300px;">
            <div class="modal-container" style="height:100%;">
                <h2>{{this.Header}}</h2>
                <h3>{{this.projectName}}</h3>
                <div>
                    <button @click="$emit('confirm', object.guid)" v-for="object in objects" :key="object.guid">{{object.name}}</button>
                </div>
                <button @click="$emit('cancel')">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    export default defineComponent({
        props: ['project', 'Header'],
        emits :['update:modelValue'],
        data() {
            return {
                projectName: "",
                objects: []
            };
        },
        components: {
        },
        created() {
            this.fetchData();
        },
        watch: {
        },
        methods: {
            fetchData(){{
                fetch('/api/project/' + this.project + '/name')
                    .then(r => r.text())
                    .then(text => {
                        this.projectName = text;
                        return;
                    });

                fetch('/api/project/' + this.project + '/objects')
                    .then(r => r.json())
                    .then(json => {
                        this.objects = json;
                        return;
                    });
            }}
        },
    });
</script>

<style>
    .modal-container{
        color:black;
    }
</style>