<template>
    <div class="post">
        <div v-if="editing">
            <table style="width:100%;">
                    <tbody>
                        <tr v-for="field in editing.properties" :key="field">
                            <td style="width:10%; vertical-align:middle;"><b>{{ field.name }}</b></td>
                            <td style="float:left; width:90%" v-if="!field.isArray">
                                <component :is="field.renderComponent" v-model="field.value"
                                           @update:model-value="updateField(project, id, field.name, field.value)" />
                            </td>
                            <td v-else>
                                <button class="compose-button" @click="field.value.push(null)">+</button>
                                <component v-for="(column, index) in field.value" :key="index" :is="field.renderComponent" v-model="field.value[index]"
                                           @update:model-value="updateField(project, id, field.name, field.value)" style="margin-bottom:4px" />
                            </td>

                        </tr>
                </tbody>
            </table>
            <div v-if="!hideJSON">
                <input type="text" :value="JSON.stringify(this.editing)" readonly style="width:100%;height:100px;" />
            </div>
        </div>
        <div v-else>
            <p>Object null :(</p>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    import FC_Default from './FieldComponents/FC_Default.vue'
    import FC_RichText from './FieldComponents/FC_RichText.vue'
    import FC_Code from './FieldComponents/FC_Code.vue'
    import FC_Reference from './FieldComponents/FC_Reference.vue'
    import FC_Toggle from './FieldComponents/FC_Toggle.vue'
    import FC_Number from './FieldComponents/FC_Number.vue'
    import FC_InlineReference from './FieldComponents/FC_InlineReference.vue'

    export default defineComponent({
        props: ['editing', 'hideJSON'],
        data() {
            return {
                DisplayNameIndex: null
            };
        },
        computed: {
            objectName: function () {
                const propertyIndex = this.editing.properties.findIndex(prop => prop.name === this.post.nameField);
                if (propertyIndex == -1) return "new object";
                return this.editing.properties[propertyIndex].value;
            }
        },
        components: {
            FC_Default,
            FC_RichText,
            FC_Code,
            FC_Reference,
            FC_Toggle,
            FC_Number,
            FC_InlineReference
        },
        created() {  
        },
        watch: {
        },
        methods: {
            updateField(project, id, fieldname, value) {
                //connection.invoke('UpdateField', project, id, fieldname, value);
            },
            updateFieldFromOther(fieldname, value) {
                //var propertyIndex = this.post.properties.findIndex(prop => prop.name === fieldname);
                //this.post.properties[propertyIndex].value = value;
            }
        },
    });
</script>