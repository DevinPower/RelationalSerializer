<template>
    <InitialSetupModal :open="showSetupModal" @close="showSetupModal = false" />

    <div v-if="loaded" class="flex min-h-screen w-full">

        <ProjectNav @update:project="swapProject" :selectedIndex="project" />
        <div style="width:250px; padding:16px;">
          <ObjectNav :project="project" />
        </div>


        <main class="flex-5 flex flex-col min-h-screen">
            <div class="router-view-container grow flex flex-col">
                <router-view class="router-view-content grow" />
            </div>
        </main>
    </div>
</template>

<script>
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
      project: 0,
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
  methods: {
    swapProject(index) {
      this.project = index;
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

