<template>
    <InitialSetupModal :open="showSetupModal" @close="showSetupModal = false" />

    <div v-if="loaded" class="flex min-h-screen w-full">

        <div style="width:200px;">
          <ProjectNav @update:project="swapProject" :selectedIndex="route.params.project" />
        </div>

        <div style="padding:16px; width:250px;">
          <ObjectNav v-if="route" :project="route.params.project" :id="route.params.id"/>
        </div>


        <main class="flex-5 flex flex-col min-h-screen">
            <div class="router-view-container grow flex flex-col">
                <router-view class="router-view-content grow" />
            </div>
        </main>
    </div>
</template>

<script>
    import { useRoute } from 'vue-router';
    import ObjectNav from './components/ObjectNav.vue';
    import ProjectNav from './components/ProjectNav.vue';
    import BreadcrumbNav from './components/BreadcrumbNav.vue';
    import InitialSetupModal from './components/InitialSetupModal.vue';

  export default {
    name: 'App',
    components: {
      ObjectNav,
      ProjectNav,
      BreadcrumbNav,
      InitialSetupModal
    },
    data() {
      return {
        showSetupModal: false,
        loaded: false
      };
    },
    created(){
      fetch('/api/onboard/' )
      .then(r => r.text())
      .then(text => {
          this.loaded = true;
          if (text == 'true'){
            this.showSetupModal = true;
          }
          return;
      });
    },
    computed: {
      route() {
        return useRoute();
      }
    },
    methods: {
      swapProject(index) {
        this.$router.push({
          //TODO: Consider we don't want to keep the active object active
          path: `/edit/${index}/${this.route.params.id}`
        });
      }
    }
  }
</script>

<style>
  .router-view-container {
    flex: 1 1 0%;
    display: flex;
    flex-direction: column;
    min-height: 0;
  }
  .router-view-content {
    flex: 1 1 0%;
    min-height: 0;
    height: 100%;
    display: flex;
    flex-direction: column;
  }
</style>

