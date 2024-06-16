import { createApp } from 'vue';
import App from './App.vue';
import WebObject from './components/WebObject.vue';
import Wizard from './components/Wizard.vue'
import TemplateEditor from './components/TemplateEditor.vue'
import TemplatesList from './components/TemplatesList.vue'
import { createRouter, createWebHistory } from 'vue-router';
import VueSimpleContextMenu from 'vue-simple-context-menu';
import 'vue-simple-context-menu/dist/vue-simple-context-menu.css';

const routes = [
    {
        path: '/templates/',
        component: TemplatesList
    },
    {
        path: '/template/:project',
        component: TemplateEditor,
        props: true
    },
    {
        path: '/edit/:project/:id',
        component: WebObject,
        props: true,
    },
    {
        path: '/import',
        component: Wizard
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

const app = createApp(App);
app.use(router);
app.component('vue-simple-context-menu', VueSimpleContextMenu);

app.mount('#app');