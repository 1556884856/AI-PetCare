<!--
  宠物管理页面（管理员）
  查看所有宠物信息（含已删除）
-->
<template>
  <div><h1 class="admin-title">🐾 宠物管理</h1>
    <el-table :data="list" v-loading="loading" style="width: 100%;">
      <el-table-column label="类型" width="80"><template #default="{ row }">{{ row.type === 'Cat' ? '🐈' : '🐕' }}</template></el-table-column>
      <el-table-column prop="name" label="名字" />
      <el-table-column prop="breed" label="品种" />
      <el-table-column prop="age" label="年龄(月)" width="100" />
      <el-table-column prop="weight" label="体重(kg)" width="100" />
      <el-table-column prop="notes" label="备注" />
      <el-table-column label="创建时间" width="180"><template #default="{ row }">{{ formatTime(row.createdAt) }}</template></el-table-column>
    </el-table>
  </div>
</template>
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllPets } from '@/api/pets'
const list = ref<any[]>([]); const loading = ref(false)
onMounted(async () => { loading.value = true; try { const res: any = await getAllPets(); list.value = res.data || [] } finally { loading.value = false } })
const formatTime = (t: string) => { const d = new Date(t); return d.toLocaleDateString() + ' ' + d.toLocaleTimeString() }
</script>
<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; color: #333; }
</style>
