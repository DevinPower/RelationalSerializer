<template>
        <div v-if="post" class="p-5 grid grid-cols-1 gap-x-6 gap-y-8 sm:grid-cols-6 bg-gray-100">

          <div v-for="field in post.properties" :key="field" class="sm:col-span-4">
            <div v-if="!field.isArray" class="mt-2">
                <label :for="field.name" class="block text-sm/6 font-medium text-gray-900">{{ field.name }}</label>
                <component :is="field.renderComponent" v-model="field.value"
                    :additionalData="field.additionalData"
                    @update:model-value="updateField(project, id, field.name, field.value)" 
                    :selfName="field.name" />

            </div>

            <div v-else class="mt-2">
                <div class="relative">
                    <div class="absolute inset-0 flex items-center" aria-hidden="true">
                        <div class="w-full border-t border-gray-300" />
                    </div>
                    <div class="relative flex items-center justify-between">
                        <span class="bg-gray pr-3 text-base font-semibold text-gray-900">{{ field.name }}</span>
                        <button @click="field.value.push(null)" type="button" class="inline-flex items-center gap-x-1.5 rounded-full bg-white px-3 py-1.5 text-sm font-semibold text-gray-900 shadow-xs ring-1 ring-gray-300 ring-inset hover:bg-gray-50">
                            <PlusIcon class="-mr-0.5 -ml-1 size-5 text-gray-400" aria-hidden="true" />
                        </button>
                    </div>
                </div>

                <div v-for="(column, index) in field.value" :key="index" class="p-1">

                    <button @click="removeFromArray(field.name, index)" style="float:right" type="button" class="rounded-full bg-red-600 p-1 text-white shadow-xs hover:bg-red-500 focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-red-600">
                        <MinusIcon class="size-4" aria-hidden="true" />
                    </button>

                    <component :is="field.renderComponent" v-model="field.value[index]"
                                :additionalData="field.additionalData"
                                @update:model-value="updateField(project, id, field.name + '[' + index + ']', field.value[index])"
                                style="margin-bottom:4px; width:calc(90% - 96px);margin-left:96px;" 
                                :selfName="field.name + '[' + index + ']'"/>
                </div>

            </div>

          </div>

        </div>
</template>

<script lang="js">
    import { defineComponent, computed } from 'vue';
    import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
    import {  PlusIcon, MinusIcon
        } from '@heroicons/vue/24/outline';

    import FC_Default from './FieldComponents/FC_Default.vue'
    import FC_RichText from './FieldComponents/FC_RichText.vue'
    import FC_Code from './FieldComponents/FC_Code.vue'
    import FC_TextArea from './FieldComponents/FC_TextArea.vue'
    import FC_Reference from './FieldComponents/FC_Reference.vue'
    import FC_Toggle from './FieldComponents/FC_Toggle.vue'
    import FC_Number from './FieldComponents/FC_Number.vue'
    import FC_Choice from './FieldComponents/FC_Choice.vue'
    import FC_Slider from './FieldComponents/FC_Slider.vue'
    import FC_InlineReference from './FieldComponents/FC_InlineReference.vue'

    const connection = new HubConnectionBuilder()
        .withUrl('/api/CoopHub')
        .configureLogging(LogLevel.Information)
        .build();

    connection.start();

    export default defineComponent({
        name: 'WebObject',
        props: ['id', 'project', 'isInline'],
        data() {
            return {
                loading: false,
                post: null,
                DisplayNameIndex: null
            };
        },
        computed: {
            objectName: function () {
                const propertyIndex = this.post.properties.findIndex(prop => prop.name === this.post.nameField);
                if (propertyIndex == -1) return "new object";
                return this.post.properties[propertyIndex].value;
            }
        },
        components: {
            FC_Default,
            FC_RichText,
            FC_Code,
            FC_TextArea,
            FC_Reference,
            FC_Toggle,
            FC_Number,
            FC_Choice,
            FC_Slider,
            FC_InlineReference,
            PlusIcon, MinusIcon
        },
        created() {  
            connection.on('updatefieldfromother', (fieldname, value) => {
                this.updateFieldFromOther(fieldname, value);
            });
            connection.on('updatearrayfromother', (fieldname, value, index) => {
                this.updateArrayFromOther(fieldname, value, index);
            });
            connection.on('removeFromArrayFromOther', (fieldname, index) => {
                this.removeFromArrayFromOther(fieldname, index);
            });
            //

            this.fetchData(this.project, this.id);
        },
        watch: {
            '$route.params.id': function () { 
                this.fetchData(this.$route.params.project, this.$route.params.id);
            }
        },
        methods: {
            fetchData(viewProject, viewId) { 
                this.post = null;
                this.loading = true;

                fetch('/api/object/' + viewProject + '/' + viewId + '/' )
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        this.DisplayNameIndex = this.post.properties.findIndex(prop => prop.name === this.post.DisplayName);
                        return;
                    });
            },
            updateField(project, id, fieldname, value) {
                connection.invoke('UpdateField', project, id, fieldname, value);
            },
             updateArrayFromOther(fieldname, value, index) {
                var propertyIndex = this.post.properties.findIndex(prop => prop.name === fieldname);
                this.post.properties[propertyIndex].value[index] = value;
            },
            updateFieldFromOther(fieldname, value) {
                var propertyIndex = this.post.properties.findIndex(prop => prop.name === fieldname);
                this.post.properties[propertyIndex].value = value;
            },
            removeFromArray(fieldname, index) {
                var propertyIndex = this.post.properties.findIndex(prop => prop.name === fieldname);
                this.post.properties[propertyIndex].value.splice(index, 1);
                connection.invoke('RemoveFromArray', this.project, this.id, fieldname, index);
            },
            removeFromArrayFromOther(fieldname, index) {
                var propertyIndex = this.post.properties.findIndex(prop => prop.name === fieldname);
                this.post.properties[propertyIndex].value.splice(index, 1);
            },
            instantiateInlineReference(fieldname) {
                connection.invoke('InstantiateIntoField', this.project, this.id, fieldname);
            }
        },
    });
</script>