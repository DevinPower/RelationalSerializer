<template>
    <input @click="promptReference(additionalData.referenceCategory)" :value="modelValue" 
    type="text" style="cursor:pointer;" readonly placeholder="null"  />

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
                referencePrompt: null
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
            promptReference(assignProject) {
                this.getProjectOptions();

                const thisRef = this;
                this.referencePrompt = {
                    header: "Pick an object to link.",
                    project: assignProject,
                    confirmCallback: function (guid) {
                        thisRef.referencePrompt = null;
                        thisRef.$emit('update:modelValue', guid);
                    },
                    cancelCallback: function () {
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
            }
        },
    });
</script>

<style>
</style>