<template>
    <input @click="promptReference(additionalData.referenceCategory)" :value="modelValue" 
        type="text" style="cursor:pointer;" readonly placeholder="null"  />
{{ referenceObjectName }}
    <ReferenceModal v-if="referencePrompt"
                       :Header="referencePrompt.header"
                       :project="referencePrompt.project"
                       :Options="referenceOptions"
                       @confirm="referencePrompt.confirmCallback($event)"
                       @cancel="referencePrompt.cancelCallback()" />
</template>

<script lang="js">
    import { defineComponent } from 'vue';
    import ReferenceModal from '../ReferenceModal.vue'

    export default defineComponent({
        props: ['modelValue', 'additionalData', 'selfName'],
        emits :['update:modelValue'],
        data() {
            return {
                referenceOptions: [],
                referencePrompt: null,
                referenceObjectName: ''
            };
        },
        components: {
            ReferenceModal
        },
        created() {
            this.getSelectedName(this.modelValue);
        },
        watch: {
        },
        methods: {
            promptReference(assignProject) {
                this.getProjectOptions();

                const thisRef = this;
                this.referencePrompt = {
                    header: "Pick an object to link.",
                    project: assignProject,
                    confirmCallback: (guid) => {
                        thisRef.referencePrompt = null;
                        thisRef.$emit('update:modelValue', guid);
                        this.getSelectedName(guid);
                    },
                    cancelCallback: () => {
                        thisRef.referencePrompt = null;
                    }
                }
            },
            getProjectOptions(){
                fetch('/api/project/' + this.additionalData.referenceCategory + '/objects')
                    .then(r => r.json())
                    .then(json => {
                        this.referenceOptions = json;
                        return;
                    });
            },
            getSelectedName(nameGuid){
                fetch('/api/object/' + this.additionalData.referenceCategory + '/' + nameGuid + '/name')
                    .then(r => r.text().then(textName =>{
                            this.referenceObjectName = textName;
                        }
                    ))
            },
        },
    });
</script>

<style>
</style>