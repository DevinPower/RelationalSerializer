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
                <div class="flex items-center my-4">
                    <div class="relative flex items-center justify-between">
                        <span class="pr-3 text-base font-semibold text-gray-900">{{ field.name }}</span>
                    </div>
                    <div class="flex-grow border-t border-gray-300" />
                </div>


                <div class="px-0 sm:px-6 lg:px-0">
                    <div class="mt-8 flow-root">
                    <div class="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
                        <div class="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
                        <div class="overflow-hidden shadow-sm ring-1 ring-black/5 sm:rounded-lg">
                            <table class="min-w-full divide-y divide-gray-300">
                            <thead class="bg-gray-50">
                                <tr>
                                <th scope="col" class="py-3.5 pr-3 pl-4 text-left text-sm font-semibold text-gray-900 sm:pl-6">Value</th>
                                <th scope="col" class="px-3 py-3.5 text-left text-sm font-semibold text-gray-900 text-right">Manage</th>
                                </tr>
                            </thead>
                            <tbody class="divide-y divide-gray-200 bg-white">
                                <tr v-for="(column, index) in field.value" :key="index">
                                <td class="py-4 w-full pr-3 pl-4 text-sm font-medium whitespace-nowrap text-gray-900 sm:pl-6">
                                    <component :is="field.renderComponent" v-model="field.value[index]"
                                        :additionalData="field.additionalData"
                                        @update:model-value="updateField(project, id, field.name + '[' + index + ']', field.value[index])"
                                        :selfName="field.name + '[' + index + ']'"/>
                                </td>
                                <td class="relative py-4 pr-4 pl-3 text-right text-sm font-medium whitespace-nowrap sm:pr-6">
                                    <span class="isolate inline-flex rounded-md shadow-xs">
                                        <button @click="removeFromArray(field.name, index)" type="button" class="relative inline-flex items-center rounded-md bg-red-200 px-3 py-2 text-sm font-semibold text-white-900 ring-1 ring-red-300 ring-inset hover:bg-red-500 focus:z-10"><MinusIcon class="size-4" aria-hidden="true" /></button>
                                    </span>
                                </td>
                                </tr>
                                <tr>
                                    <td @click="field.value.push(null)" class="bg-gray-200 hover:bg-gray-300 p-2 text-gray-500" colspan="2" style="cursor: pointer;">
                                        <center><PlusIcon class="size-6" aria-hidden="true" /></center>
                                    </td>
                                </tr>
                            </tbody>
                            </table>
                        </div>
                        </div>
                    </div>
                    </div>
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

            if (this.$route.params.project && this.$route.params.id)
                this.fetchData(this.$route.params.project, this.$route.params.id);
        },
        watch: {
            '$route.params.id': function () { 
                if (this.$route.params.id == 'undefined'){
                    this.post = null;
                    return;
                }

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