import { GET_PAGINATION_HEADERS, PAGINATION_ERROR, RESET_PAGINATION } from '../actions/types';

const initialState = {
	loading: true,
	error: {},
	currentPage: null,
	itemsPerPage: null,
	totalItems: null,
	totalPages: null
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case GET_PAGINATION_HEADERS:
			return {
				...state,
				loading: false,
				currentPage: payload.currentPage,
				itemsPerPage: payload.itemsPerPage,
				totalItems: payload.totalItems,
				totalPages: payload.totalPages
			};
		// case PAGINATION_ERROR:
		// 	return {
		// 		...state,
		// 		loading: false,
		// 		error: payload
		// 	};
		case RESET_PAGINATION:
			return {
				...state,
				loading: false,
				currentPage: 1
			};
		default:
			return state;
	}
}
