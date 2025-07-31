<template>
        <div class="mt-2">
            <div class="flex items-center my-4">
                <div style="width:400px"
                    class="border-b bg-white border-gray-200 shadow-sm p-1 pb-px focus-within:border-b-2 focus-within:border-indigo-600 focus-within:pb-0">
                        <input rows="3" class="block w-full resize-none text-base text-gray-900 placeholder:text-gray-400 focus:outline-none sm:text-sm/6" 
                        @click="promptReference(additionalData.referenceCategory)" :value="modelValue" 
                        type="text" style="cursor:pointer;" readonly placeholder="null"  />
            </div>
                
            <div style="padding-left:12px;">
                <RouterLink :to="`/edit/${additionalData.referenceCategory}/${modelValue}`">{{ referenceObjectName }}</RouterLink>
            </div>
        </div>
    </div>

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
                if (this.referencePrompt != null){
                    this.referencePrompt.cancelCallback();
                    return;
                }

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