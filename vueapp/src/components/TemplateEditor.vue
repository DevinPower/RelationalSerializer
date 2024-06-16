<template>
    <div class="post">
        <h1>Templates/{{this.project}}</h1>

        <div v-if="!loading" class="content">
            <table style="width:100%;" >
                <tbody>
                    <tr v-for="property in post.properties" :key="property.name">

                    <td style="width:200px; vertical-align:top;">{{property.name}}</td>

                    <td><select @change="addModifier($event, property.name)">
                        <option> </option>
                        <option v-for="modifier in property.availableModifiers" :key="modifier.name">{{modifier.name}}</option>
                        </select>

                        <div v-for="modifier in property.activeModifiers" :key="modifier.name">
                            <h3>{{modifier.name}} <button style="display:inline" @click="removeModifier(modifier.name, property.name)">X</button></h3>
                            <ObjectEditor :editing="modifier.underlyingObject" hideJSON="true" />
                        </div>
                    </td>

                </tr>
                </tbody>
            </table>

            <button @click="patchTemplate()">PATCH</button>
        </div>
    </div>
</template>

<script lang="js">
    import { defineComponent } from 'vue';

    import ObjectEditor from './ObjectEditor.vue'

    export default defineComponent({
        props: ['project'],
        data() {
            return {
                loading: true,
                post: null,
                DisplayNameIndex: null
            };
        },
        computed: {
        },
        components: {
            ObjectEditor
        },
        created() {  
                this.fetchData(this.$route.params.project);
        },
        watch: {
            '$route.params.id': function () { 
                this.fetchData(this.$route.params.project);
            }
        },
        methods: {
            fetchData(viewProject) { 
                this.post = null;
                this.loading = true;

                fetch('/api/object/' + viewProject + '/template' )
                    .then(r => r.json())
                    .then(json => {
                        this.post = json;
                        this.loading = false;
                        return;
                    });
            },
            patchTemplate(){

                fetch('/api/object/' + this.$route.params.project + '/template', {
                    method: "PATCH",
                    body: JSON.stringify(this.post.properties),
                    headers: { "Content-Type": "application/json" }
                }).then((response) => response.text())
                    .then((x) => alert(x));
            },
            addModifier(event, name){
                if (event.target.value == " ") return;

                const field = this.post.properties.find((x) => x.name == name);

                if (field.activeModifiers == null) field.activeModifiers = [];

                field.activeModifiers.push(field.availableModifiers.find((x) => x.name == event.target.value));
                field.availableModifiers.splice(field.availableModifiers.indexOf(name), 1);
            },
            removeModifier(propName, name) {
                const field = this.post.properties.find((x) => x.name == name);

                if (field.availableModifiers == null) field.availableModifiers = [];

                field.availableModifiers.push(field.activeModifiers.find((x) => x.name == propName));
                field.activeModifiers.splice(field.activeModifiers.indexOf(name), 1);
            }
        },
    });
</script>