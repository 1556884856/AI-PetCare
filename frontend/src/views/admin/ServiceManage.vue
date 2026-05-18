<template>
  <div>
    <div class="admin-header">
      <h1>🛁 服务管理</h1>
      <el-button type="primary" @click="openDialog()">+ 添加服务</el-button>
    </div>
    <el-table :data="list" v-loading="loading" style="width: 100%">
      <el-table-column label="图标" width="80">
        <template #default="{ row }">{{ row.petType === 'Cat' ? '🐈' : row.petType === 'Dog' ? '🐕' : '🐾' }}</template>
      </el-table-column>
      <el-table-column prop="name" label="名称" />
      <el-table-column label="分类" width="100">
        <template #default="{ row }">
          <el-tag size="small">{{ cLabel(row.category) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="适用" width="100">
        <template #default="{ row }">{{ pLabel(row.petType) }}</template>
      </el-table-column>
      <el-table-column label="时长" width="100"><template #default="{ row }">{{ row.durationMinutes }}分钟</template></el-table-column>
      <el-table-column label="价格" width="100"><template #default="{ row }">¥{{ row.price }}</template></el-table-column>
      <el-table-column label="状态" width="80">
        <template #default="{ row }"><el-switch :model-value="row.isActive" @change="toggleActive(row)" /></template>
      </el-table-column>
      <el-table-column label="操作" width="150">
        <template #default="{ row }">
          <el-button size="small" @click="openDialog(row)">编辑</el-button>
          <el-button size="small" type="danger" @click="removeOne(row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="showDialog" :title="editingId ? '编辑服务' : '添加服务'" width="480px">
      <el-form :model="form" label-position="top">
        <el-form-item label="名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="描述"><el-input v-model="form.description" type="textarea" /></el-form-item>
        <el-form-item label="分类">
          <el-select v-model="form.category">
            <el-option label="洗浴" value="Bath" /><el-option label="美容" value="Grooming" />
            <el-option label="SPA" value="Spa" /><el-option label="基础护理" value="Basic" />
          </el-select>
        </el-form-item>
        <el-form-item label="适用宠物">
          <el-radio-group v-model="form.petType">
            <el-radio value="All">猫狗通用</el-radio><el-radio value="Dog">仅狗狗</el-radio><el-radio value="Cat">仅猫咪</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-row :gutter="16">
          <el-col :span="12"><el-form-item label="价格"><el-input-number v-model="form.price" :min="0" :step="10" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="时长（分钟）"><el-input-number v-model="form.durationMinutes" :min="10" :step="5" /></el-form-item></el-col>
        </el-row>
        <el-form-item label="排序"><el-input-number v-model="form.sortOrder" :min="0" /></el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="saveService">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getServices, createService, updateService, deleteService } from '@/api/services'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const loading = ref(false)
const showDialog = ref(false)
const editingId = ref<number | null>(null)
const form = reactive({ name: '', description: '', category: 'Bath', petType: 'All', price: 100, durationMinutes: 60, sortOrder: 0 })

const cLabel = (c: string) => ({ Bath: '洗浴', Grooming: '美容', Spa: 'SPA', Basic: '基础护理' } as any)[c] || c
const pLabel = (p: string) => ({ All: '猫狗通用', Dog: '狗狗', Cat: '猫咪' } as any)[p] || p

const resetForm = () => { editingId.value = null; Object.assign(form, { name: '', description: '', category: 'Bath', petType: 'All', price: 100, durationMinutes: 60, sortOrder: 0 }) }
const openDialog = (row?: any) => {
  if (row) { editingId.value = row.id; Object.assign(form, row) } else { resetForm() }
  showDialog.value = true
}

const fetchList = async () => { loading.value = true; try { const res: any = await getServices(); list.value = res.data || [] } catch {} finally { loading.value = false } }

const saveService = async () => {
  try {
    if (editingId.value) { await updateService(editingId.value, form) } else { await createService(form) }
    ElMessage.success('保存成功'); showDialog.value = false; fetchList()
  } catch {}
}

const toggleActive = async (row: any) => { try { await updateService(row.id, { ...row, isActive: !row.isActive }); fetchList() } catch {} }

const removeOne = async (id: number) => {
  await ElMessageBox.confirm('确定删除？', '提示', { type: 'warning' })
  try { await deleteService(id); ElMessage.success('已删除'); fetchList() } catch {}
}

onMounted(fetchList)
</script>

<style scoped>
.admin-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.admin-header h1 { font-size: 24px; }
</style>
