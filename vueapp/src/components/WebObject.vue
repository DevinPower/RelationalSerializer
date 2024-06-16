<template>
    <div class="post">
        <div v-if="post" class="content">
            <h3 v-if="project && !isInline">{{ this.project }}/{{ this.id }}/{{objectName}}</h3>

            <table style="width:100%;">
                <tbody>
                    <tr v-for="field in post.properties" :key="field">
                        <td style="width: 10%; vertical-align: top; padding-bottom: 1em;"><b>{{ field.name }}</b><div class="vl"></div></td>
                        <td style="float: left; width: 90%; padding-bottom: 1em;" v-if="!field.isArray">
                            <component :is="field.renderComponent" v-model="field.value"
                                       :additionalData="field.additionalData"
                                       @update:model-value="updateField(project, id, field.name, field.value)" 
                                       :selfName="field.name" />
                        </td>
                        <td v-else style="padding-bottom: 1em;">
                            <button class="compose-button" style="float:left;margin-right:-48px;width:48px;" @click="field.value.push(null)">+</button>
                            <div v-for="(column, index) in field.value" :key="index">
                                <button class="compose-button red" style="width:48px;float:right;" @click="removeFromArray(field.name, index)">-</button>

                                <component :is="field.renderComponent" v-model="field.value[index]"
                                           :additionalData="field.additionalData"
                                           @update:model-value="updateField(project, id, field.name + '[' + index + ']', field.value[index])"
                                           style="margin-bottom:4px; width:calc(90% - 96px);margin-left:96px;" 
                                           :selfName="field.name + '[' + index + ']'"/>
                            </div>
                        </td>

                    </tr>
                </tbody>
            </table>
            <!--<textarea type="text" readonly style="width:100%;height:100px;">
        {{JSON.stringify(this.post, null, 4)}}
    </textarea>-->
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent, computed } from 'vue';
    import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

    import FC_Default from './FieldComponents/FC_Default'
    import FC_RichText from './FieldComponents/FC_RichText'
    import FC_Code from './FieldComponents/FC_Code'
    import FC_Reference from './FieldComponents/FC_Reference'
    import FC_Toggle from './FieldComponents/FC_Toggle'
    import FC_Number from './FieldComponents/FC_Number'
    import FC_Choice from './FieldComponents/FC_Choice'
    import FC_Slider from './FieldComponents/FC_Slider'
    import FC_InlineReference from './FieldComponents/FC_InlineReference'

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
            FC_Reference,
            FC_Toggle,
            FC_Number,
            FC_Choice,
            FC_Slider,
            FC_InlineReference
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

<style>
    .red {
        background-color: #e74c3c;
    }

        .red:hover {
            background-color: #c0392b;
        }
</style>