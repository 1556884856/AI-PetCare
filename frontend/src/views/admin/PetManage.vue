<template>
  <div>
    <h1 class="admin-title">🐾 宠物管理</h1>
    <el-table :data="list" v-loading="loading" style="width: 100%">
      <el-table-column label="宠物">
        <template #default="{ row }">{{ row.type === 'Cat' ? '🐈' : '🐕' }} {{ row.name }}</template>
      </el-table-column>
      <el-table-column prop="breed" label="品种" />
      <el-table-column label="类型" width="80">
        <template #default="{ row }">{{ row.type === 'Cat' ? '猫咪' : '狗狗' }}</template>
      </el-table-column>
      <el-table-column label="年龄" width="100"><template #default="{ row }">{{ row.age }}个月</template></el-table-column>
      <el-table-column label="体重" width="100"><template #default="{ row }">{{ row.weight }}kg</template></el-table-column>
      <el-table-column prop="ownerPhone" label="主人" width="140" />
      <el-table-column prop="notes" label="备注" />
    </el-table>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllPets } from '@/api/pets'

const list = ref<any[]>([])
const loading = ref(false)

onMounted(async () => {
  loading.value = true
  try { const res: any = await getAllPets(); list.value = res.data || [] } catch {} finally { loading.value = false }
})
</script>

<style scoped>
.admin-title { font-size: 24px; margin-bottom: 24px; }
</style>
