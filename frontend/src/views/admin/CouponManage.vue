<template>
  <div class="page">
    <div class="page-header">
      <h2>🎫 优惠券管理</h2>
      <el-button type="primary" @click="showDialog = true; editing = null; resetForm()">创建优惠券</el-button>
    </div>

    <el-table :data="list" stripe>
      <el-table-column prop="name" label="券名称" />
      <el-table-column label="类型" width="80">
        <template #default="{ row }">{{ row.type === 0 ? '满减' : '折扣' }}</template>
      </el-table-column>
      <el-table-column label="面值" width="100">
        <template #default="{ row }">
          <template v-if="row.type === 0">¥{{ row.value }}</template>
          <template v-else>{{ (row.value * 10).toFixed(1) }}折</template>
        </template>
      </el-table-column>
      <el-table-column prop="minOrderAmount" label="最低消费" width="90">
        <template #default="{ row }">¥{{ row.minOrderAmount }}</template>
      </el-table-column>
      <el-table-column label="有效期" width="200">
        <template #default="{ row }">{{ formatDate(row.validFrom) }} ~ {{ formatDate(row.validTo) }}</template>
      </el-table-column>
      <el-table-column label="发放/总量" width="90">
        <template #default="{ row }">{{ row.usedCount }}/{{ row.totalQuantity }}</template>
      </el-table-column>
      <el-table-column prop="isActive" label="状态" width="70">
        <template #default="{ row }">
          <el-tag :type="row.isActive ? 'success' : 'info'" size="small">{{ row.isActive ? '启用' : '停用' }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作" width="180">
        <template #default="{ row }">
          <el-button type="primary" link size="small" @click="edit(row)">编辑</el-button>
          <el-button type="warning" link size="small" @click="showDistribute(row)">派发</el-button>
          <el-button type="danger" link size="small" @click="remove(row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="showDialog" :title="editing ? '编辑优惠券' : '创建优惠券'" width="500px">
      <el-form :model="form" label-position="top">
        <el-form-item label="券名称"><el-input v-model="form.name" /></el-form-item>
        <el-form-item label="类型">
          <el-radio-group v-model="form.type">
            <el-radio :value="0">满减</el-radio>
            <el-radio :value="1">折扣（0.85=85折）</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="面值">
          <el-input-number v-model="form.value" :min="0" :step="form.type === 0 ? 5 : 0.05" />
        </el-form-item>
        <el-form-item label="最低消费">
          <el-input-number v-model="form.minOrderAmount" :min="0" :step="10" />
        </el-form-item>
        <el-form-item label="有效期起">
          <el-date-picker v-model="form.validFrom" type="date" />
        </el-form-item>
        <el-form-item label="有效期止">
          <el-date-picker v-model="form.validTo" type="date" />
        </el-form-item>
        <el-form-item label="发放总量">
          <el-input-number v-model="form.totalQuantity" :min="1" :step="100" />
        </el-form-item>
        <el-form-item v-if="editing" label="启用">
          <el-switch v-model="form.isActive" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="showDialog = false">取消</el-button>
        <el-button type="primary" @click="save">{{ editing ? '保存' : '创建' }}</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="showDistDialog" title="派发优惠券" width="400px">
      <p style="margin-bottom: 16px;">选择要派发的用户（多选）</p>
      <el-select v-model="distUserIds" multiple placeholder="选择用户" style="width: 100%;" filterable>
        <el-option v-for="u in customers" :key="u.id" :label="u.nickname + ' (' + u.phone + ')'" :value="u.id" />
      </el-select>
      <template #footer>
        <el-button @click="showDistDialog = false">取消</el-button>
        <el-button type="primary" @click="doDistribute">确认派发</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getAllCoupons, createCoupon, updateCoupon, deleteCoupon, distributeCoupon } from '@/api/coupons'
import { getCustomers } from '@/api/admin'
import { ElMessage, ElMessageBox } from 'element-plus'

const list = ref<any[]>([])
const customers = ref<any[]>([])
const showDialog = ref(false)
const editing = ref<any>(null)
const form = ref({ name: '', type: 0, value: 20, minOrderAmount: 0, validFrom: '', validTo: '', totalQuantity: 100, isActive: true })
const showDistDialog = ref(false)
const distCouponId = ref(0)
const distUserIds = ref<number[]>([])

const load = async () => {
  try {
    const res: any = await getAllCoupons()
    list.value = res.data || []
    const cRes: any = await getCustomers()
    customers.value = cRes.data || []
  } catch {}
}

onMounted(load)

const resetForm = () => {
  form.value = { name: '', type: 0, value: 20, minOrderAmount: 0, validFrom: '', validTo: '', totalQuantity: 100, isActive: true }
}

const edit = (row: any) => {
  editing.value = row
  form.value = {
    name: row.name,
    type: row.type,
    value: row.value,
    minOrderAmount: row.minOrderAmount,
    validFrom: row.validFrom,
    validTo: row.validTo,
    totalQuantity: row.totalQuantity,
    isActive: row.isActive
  }
  showDialog.value = true
}

const save = async () => {
  try {
    if (editing.value) {
      await updateCoupon(editing.value.id, form.value)
      ElMessage.success('已更新')
    } else {
      await createCoupon(form.value)
      ElMessage.success('已创建')
    }
    showDialog.value = false
    load()
  } catch {}
}

const remove = async (id: number) => {
  try {
    await ElMessageBox.confirm('确定删除此优惠券？', '确认', { type: 'warning' })
    await deleteCoupon(id)
    ElMessage.success('已删除')
    load()
  } catch {}
}

const showDistribute = (row: any) => {
  distCouponId.value = row.id
  distUserIds.value = []
  showDistDialog.value = true
}

const doDistribute = async () => {
  try {
    await distributeCoupon(distCouponId.value, { userIds: distUserIds.value })
    ElMessage.success('派发成功')
    showDistDialog.value = false
  } catch {}
}

const formatDate = (t: string) => {
  if (!t) return ''
  return new Date(t).toLocaleDateString()
}
</script>

<style scoped>
.page { padding: 24px; }
.page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.page-header h2 { font-size: 20px; }
</style>
